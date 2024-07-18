class BookingRequest{
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
            return this.userInfo;
        } catch (error) {
            console.error('Fetch request failed:', error);
            throw error;
        }
    }

    async addBookingRequest() {
        event.preventDefault();
        try {
            let reservationDate = new Date(document.getElementById('datetime').value).toISOString();
            let numberOfGuests = parseInt(document.getElementById('numberOfPeople').value);
            let status = 'Pending';
            let note = document.getElementById('message').value;
            let userId = this.userInfo.userId;
            let tableSelect = document.getElementById('tableSelect');
            let tableId = parseInt(tableSelect.value);
            debugger
            let bookingRequest = {
                reservationDate: reservationDate,
                numberOfGuests: numberOfGuests,
                status: status,
                note: note,
                userId: userId,
                tableId: tableId
            };

            const response = await fetch('https://localhost:5000/api/BookingRequest/AddBookingRequestAsync', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwtToken
                },
                body: JSON.stringify(bookingRequest)
            });

            if (!response.ok) {
                throw new Error('Failed to add booking request');
            }

            const result = await response.json();
            let tableSelectContainer = document.getElementById('tableSelectContainer');
            alert('Booking successfully');
            document.getElementById('datetime').value = '';
            document.getElementById('numberOfPeople').value = '';
            document.getElementById('message').value = '';
            tableSelectContainer.style.display = 'none';
        } catch (error) {
            console.error('Error adding booking request:', error);
            alert('Failed to add booking request. Please try again later.');
        }
    }
}
var bookingRequest = new BookingRequest();

async function showTableSelect() {
    try {
        let numberOfPeople = document.getElementById('numberOfPeople').value;
        let tableSelectContainer = document.getElementById('tableSelectContainer');
        let tableSelect = document.getElementById('tableSelect');

        if (numberOfPeople) {
            const response = await fetch('https://localhost:5000/api/BookingRequest/getTableByNumOfPeople', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + bookingRequest.jwtToken
                },
                body: JSON.stringify(parseInt(numberOfPeople))
            });

            if (!response.ok) {
                throw new Error('Failed to fetch tables');
            }

            const tables = await response.json();

            tableSelect.innerHTML = '';

            tables.forEach(table => {
                let option = document.createElement('option');
                option.value = table.tableId;
                option.text = `Table ${table.tableId} - Capacity: ${table.capacity}`;
                tableSelect.appendChild(option);
            });

            tableSelectContainer.style.display = 'block';
        } else {
            tableSelectContainer.style.display = 'none';
        }
    } catch (error) {
        console.error('Error fetching tables:', error);
        alert('Failed to fetch tables. Please try again later.');
    }
}
