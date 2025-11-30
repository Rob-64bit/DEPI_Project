// rooms-filter.js
document.addEventListener('DOMContentLoaded', function () {
    const checkinInput = document.getElementById('checkin');
    const checkoutInput = document.getElementById('checkout');
    const guestsSelect = document.querySelector('select');
    const searchBtn = document.querySelector('.filter-btn');
    const sortSelect = document.querySelector('.sort-by');
    const filterButtons = document.querySelectorAll('.filter-option');

    // Search Button Click
    if (searchBtn) {
        searchBtn.addEventListener('click', function () {
            const checkin = checkinInput?.value;
            const checkout = checkoutInput?.value;
            const guests = guestsSelect?.value;
            const sort = sortSelect?.value;

            // بناء الـ URL
            let url = '/Rooms/Index?';
            if (checkin) url += `checkin=${checkin}&`;
            if (checkout) url += `checkout=${checkout}&`;
            if (guests) url += `guests=${guests}&`;
            if (sort && sort !== 'low-to-high') url += `sort=${sort}&`;

            // Redirect للصفحة مع الـ parameters
            window.location.href = url;
        });
    }

    // Filter Buttons Click
    filterButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const filter = this.getAttribute('data-filter');
            const checkin = checkinInput?.value;
            const checkout = checkoutInput?.value;

            let url = `/Rooms/Index?filter=${filter}`;
            if (checkin) url += `&checkin=${checkin}`;
            if (checkout) url += `&checkout=${checkout}`;

            window.location.href = url;
        });
    });

    // Sort Dropdown Change
    if (sortSelect) {
        sortSelect.addEventListener('change', function () {
            const sort = this.value;
            const checkin = checkinInput?.value;
            const checkout = checkoutInput?.value;

            let url = `/Rooms/Index?sort=${sort}`;
            if (checkin) url += `&checkin=${checkin}`;
            if (checkout) url += `&checkout=${checkout}`;

            window.location.href = url;
        });
    }

    // Calculate nights display
    function updateNightsDisplay() {
        const nightsDisplay = document.getElementById('nights-display');
        if (!checkinInput || !checkoutInput || !nightsDisplay) return;

        const ci = checkinInput.value;
        const co = checkoutInput.value;

        if (ci && co) {
            const d1 = new Date(ci);
            const d2 = new Date(co);
            const diff = Math.ceil((d2 - d1) / (1000 * 60 * 60 * 24));

            if (diff > 0) {
                nightsDisplay.textContent = `${diff} night${diff > 1 ? 's' : ''}`;
                nightsDisplay.style.color = '#B29575';
            } else {
                nightsDisplay.textContent = 'Invalid dates';
                nightsDisplay.style.color = 'red';
            }
        } else {
            nightsDisplay.innerHTML = '&nbsp;';
        }
    }

    if (checkinInput) checkinInput.addEventListener('change', updateNightsDisplay);
    if (checkoutInput) checkoutInput.addEventListener('change', updateNightsDisplay);
});