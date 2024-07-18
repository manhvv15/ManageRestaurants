class UserForAdmin {
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
            if (this.userInfo.role !== "Admin") {
                $('#createUserBtn').addClass('d-none');
                $('#actionForAdmin').addClass('d-none');
            }
            return this.userInfo;
        } catch (error) {
            console.error('Fetch request failed:', error);
            throw error;
        }
    }

    async loadUsers() {
        try {
            await this.getUserInfo();

            const response = await fetch('https://localhost:5000/api/User/GetAll', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${this.jwtToken}`
                }
            });

            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }

            const users = await response.json();

            const userListTable = document.getElementById('userListTable');
            const tbody = userListTable.querySelector('tbody');
            tbody.innerHTML = ''; // Clear existing table body content

            users.forEach(user => {
                const row = `<tr>
                        <td>${user.userName}</td>
                        <td>${user.email}</td>
                        <td>${user.role}</td>
                        <td>${user.balance ?? ''}</td>
                        <td>${user.phone ?? ''}</td>
                        ${this.userInfo.role === 'Admin' ? `
                        <td>
                            <button class="btn btn-sm btn-primary" onclick="userForAdmin.openEditModal(${user.userId})" data-bs-toggle="modal" data-bs-target="#editUserModal">Edit</button>
                            <button class="btn btn-sm btn-danger" onclick="userForAdmin.deleteUser(${user.userId})">Delete</button>
                        </td>
                        ` : ''}
                    </tr>`;
                tbody.innerHTML += row;
            });
        } catch (error) {
            console.error('Error fetching users:', error);
        }
    }

    async addUser() {
        const username = document.getElementById('addUsername').value;
        const email = document.getElementById('addEmail').value;
        const password = document.getElementById('addPassword').value;
        const role = document.querySelector('input[name="addRole"]:checked').value;
        const phoneNumber = document.getElementById('addPhoneNumber').value;

        try {
            const newUser = {
                userName: username,
                email: email,
                password: password,
                createdBy: 'System',
                role: role,
                phoneNumber: phoneNumber
            };

            const response = await fetch('https://localhost:5000/api/User/CreateUser', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.jwtToken}`
                },
                body: JSON.stringify(newUser)
            });

            if (!response.ok) {
                throw new Error('Failed to add user');
            }

            alert('User added successfully');
            $('#addUserModal').modal('hide');
            this.loadUsers();
        } catch (error) {
            console.error('Error adding user:', error);
            alert('Failed to add user. Please try again later.');
        }
    }

    async getUserById(userId) {
    try {
        const response = await fetch(`https://localhost:5000/api/User/GetUserById`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwtToken
            },
            body: JSON.stringify(userId)
        });

        if (!response.ok) {
            throw new Error('Failed to fetch user details');
        }

        const userData = await response.json();
        return userData;
    } catch (error) {
        console.error('Error fetching user details:', error);
        throw error; // Throwing error để xử lý lỗi ở nơi gọi hàm này
    }
}

    async openEditModal(userId) {
        try {
            const userData = await this.getUserById(userId);
            document.getElementById('editUserId').value = userData.userId;
            document.getElementById('editUsername').value = userData.userName;
            document.getElementById('editEmail').value = userData.email;
            document.getElementById('editPassword').value = '';
            document.getElementById('editPhoneNumber').value = userData.phoneNumber ?? '';

            // Chọn radio theo role
            if (userData.role === 'Admin') {
                document.getElementById('editRoleAdmin').checked = true;
            } else {
                document.getElementById('editRoleUser').checked = true;
            }

        } catch (error) {
            console.error('Error opening edit modal:', error);
            alert('Failed to open edit modal. Please try again later.');
        }
    }

    async saveUserChanges() {
        const userId = document.getElementById('editUserId').value;
        const username = document.getElementById('editUsername').value;
        const email = document.getElementById('editEmail').value;
        const password = document.getElementById('editPassword').value;
        const role = document.querySelector('input[name="editRole"]:checked').value;
        const phoneNumber = document.getElementById('editPhoneNumber').value;

        try {
            const updatedUser = {
                userName: username,
                email: email,
                password: password,
                role: role,
                phone: phoneNumber
            };

            const response = await fetch(`https://localhost:5000/api/User/UpdateUser/${userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.jwtToken}`
                },
                body: JSON.stringify(updatedUser)
            });

            if (!response.ok) {
                throw new Error('Failed to update user');
            }

            alert('User updated successfully');
            $('#editUserModal').modal('hide');
            this.loadUsers();
        } catch (error) {
            console.error('Error updating user:', error);
            alert('Failed to update user. Please try again later.');
        }
    }

    async deleteUser(userId) {
        try {
            const response = await fetch(`https://localhost:5000/api/User/DeleteUser`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwtToken
                },
                body: JSON.stringify(userId)
            });

            if (!response.ok) {
                throw new Error('Failed to delete user');
            }

            alert('User deleted successfully');
            this.loadUsers();
        } catch (error) {
            console.error('Error deleting user:', error);
            alert('Failed to delete user. Please try again later.');
        }
    }
}
var userForAdmin = new UserForAdmin();