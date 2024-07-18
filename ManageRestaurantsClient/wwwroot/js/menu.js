class MenuView {
    constructor(...args) {
        this.initFields();
    }
    initFields() {
        this.jwtToken;
        this.userId;
        this.userInfo;
    }
    async getUserInfo() {
        try {
            const response = await fetch(`https://localhost:5000/api/User/GetUserById`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwtToken
                },
                body: JSON.stringify(this.userId)
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            this.userInfo = await response.json();
            if (this.userInfo.role !== "Admin" && this.userInfo.role !== "Staff") {
                $('#createBtn').addClass('d-none');
            }
            return this.userInfo;
        } catch (error) {
            console.error('Fetch request failed:', error);
            throw error;
        }
    }
    async getMenus() {
        await this.getUserInfo();

        try {
            const response = await fetch('https://localhost:5000/api/Menu/GetMenusAsync', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwtToken
                }
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const data = await response.json();
            var menuContainer = document.getElementById('menu-container');
            menuContainer.innerHTML = '';

            data.forEach(function (menuItem) {
                var menuHtml = `
            <div class="col-lg-6 menu-item data-menu-id="${menuItem.menuId}">
                <div class="d-flex align-items-center">
                    <img class="flex-shrink-0 img-fluid rounded" src="${menuItem.imageUrl}" alt="" style="width: 80px;">
                    <div class="w-100 d-flex flex-column text-start ps-4">
                        <h5 class="d-flex justify-content-between border-bottom pb-2">
                            <span>${menuItem.name}</span>
                            <span class="text-primary">$${menuItem.price}</span>
                            ${this.userInfo.role !== 'User' ? `
                            <div>
                                <a href="#" class="btn btn-sm btn-outline-primary me-2" onclick="menu.getDetail(${menuItem.menuId})" data-bs-toggle="modal" data-bs-target="#editItemModal">
                                    <i class="fas fa-edit"></i> Edit
                                </a>
                                <a href="#" onclick="menu.deleteItem(${menuItem.menuId})" class="btn btn-sm btn-outline-danger">
                                    <i class="fas fa-trash-alt"></i> Delete
                                </a>
                            </div>
                            ` : ''}
                        </h5>
                        <small class="fst-italic">${menuItem.description}</small>
                    </div>
                </div>
            </div>
            `;
                menuContainer.insertAdjacentHTML('beforeend', menuHtml);
            }.bind(this));
        } catch (error) {
            console.error('Fetch request failed:', error);
        }
    }

    createItem() {
        let itemName = document.getElementById('itemName').value;
        let itemDescription = document.getElementById('itemDescription').value;
        let itemPrice = document.getElementById('itemPrice').value;
        let itemImageUrl = document.getElementById('itemImageUrl').value;

        // Tạo payload dữ liệu để gửi lên server
        let payload = {
            Name: itemName,
            Description: itemDescription,
            Price: itemPrice,
            ImageUrl: itemImageUrl
        };

        fetch('https://localhost:5000/api/Menu/AddMenuAsync', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwtToken
            },
            body: JSON.stringify(payload)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                let modal = new bootstrap.Modal(document.getElementById('createItemModal'));
                modal.hide();

                this.getMenus();
            })
            .catch(error => {
                console.error('Error adding menu item:', error);
                alert('Failed to add menu item. Please try again later.');
            });
    }

    getDetail(menuId) {
        fetch(`https://localhost:5000/api/Menu/GetMenuByIdAsync`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwtToken
            },
            body: JSON.stringify(menuId)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(menuItem => {
                $('#editItemId').val(menuItem.menuId);
                $('#editItemName').val(menuItem.name);
                $('#editItemPrice').val(`${menuItem.price}`);
                $('#editItemDescription').val(menuItem.description);
                $('#editItemImageUrl').attr('src', menuItem.imageUrl).val(menuItem.imageUrl);
            })
            .catch(error => {
                console.error('Fetch request failed:', error);
            });
    }

    editItem() {
        let itemId = document.getElementById('editItemId').value;
        let itemName = document.getElementById('editItemName').value;
        let itemDescription = document.getElementById('editItemDescription').value;
        let itemPrice = document.getElementById('editItemPrice').value;
        let itemImageUrl = document.getElementById('editItemImageUrl').value;

        let payload = {
            Name: itemName,
            Description: itemDescription,
            Price: itemPrice,
            ImageUrl: itemImageUrl
        };

        fetch(`https://localhost:5000/api/Menu/UpdateMenuAsync?id=${itemId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwtToken
            },
            body: JSON.stringify(payload)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Menu item updated successfully:', data);
                $('#editItemModal').modal('hide'); // Đóng modal sau khi cập nhật thành công
                this.getMenus(); // Lấy danh sách menu mới sau khi cập nhật
            })
            .catch(error => {
                console.error('Error updating menu item:', error);
                alert('Failed to update menu item. Please try again later.');
            });
    }

    async deleteItem(menuId) {
        try {
            const response = await fetch(`https://localhost:5000/api/Menu/DeleteMenuAsync`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwtToken
                },
                body: JSON.stringify(menuId)
            });

            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            const deletedMenuItem = await response.json();

            this.getMenus();
        } catch (error) {
            console.error('Error deleting menu item:', error);
            alert('Failed to delete menu item. Please try again later.');
        }
    }
}
var menu = new MenuView();
