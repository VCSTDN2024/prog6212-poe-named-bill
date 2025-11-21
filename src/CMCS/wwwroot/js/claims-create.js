(() => {
    const form = document.getElementById('claim-create-form');
    if (!form) {
        return;
    }

    const endpoint = form.dataset.calculationEndpoint;
    const hoursInput = document.getElementById('HoursWorked');
    const rateInput = document.getElementById('HourlyRate');
    const summaryCard = document.getElementById('claim-summary');
    const totalNode = document.getElementById('claim-total');
    const warningsList = document.getElementById('claim-warning-list');
    const errorsList = document.getElementById('claim-error-list');
    const hoursNode = document.getElementById('summary-hours');
    const rateNode = document.getElementById('summary-rate');
    const indicator = summaryCard?.querySelector('.automation-indicator');

    const formatCurrency = (value) => new Intl.NumberFormat(undefined, { style: 'currency', currency: 'ZAR', minimumFractionDigits: 2 }).format(value);
    const formatNumber = (value) => new Intl.NumberFormat(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);

    const setState = (state) => {
        if (!summaryCard) return;
        summaryCard.dataset.state = state;
    };

    const clearMessages = () => {
        warningsList.innerHTML = '';
        errorsList.innerHTML = '';
    };

    const renderMessages = (items, target, cssClass) => {
        if (!target) return;
        target.innerHTML = '';
        (items || []).forEach((message) => {
            const li = document.createElement('li');
            if (cssClass) li.classList.add(cssClass);
            li.textContent = message;
            target.appendChild(li);
        });
    };

    const debounce = (fn, delay = 250) => {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => fn.apply(null, args), delay);
        };
    };

    const updateSummary = async () => {
        if (!hoursInput || !rateInput || !endpoint) return;
        const hours = parseFloat(hoursInput.value);
        const rate = parseFloat(rateInput.value);

        clearMessages();

        if (!Number.isFinite(hours) || !Number.isFinite(rate)) {
            totalNode.textContent = '--';
            hoursNode.textContent = '--';
            rateNode.textContent = '--';
            setState('empty');
            return;
        }

        if (indicator) {
            indicator.textContent = 'Calculating…';
        }

        try {
            const response = await fetch(endpoint, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'same-origin',
                body: JSON.stringify({ hoursWorked: hours, hourlyRate: rate })
            });

            if (!response.ok) {
                throw new Error('Unable to calculate summary.');
            }

            const result = await response.json();
            hoursNode.textContent = formatNumber(result.hoursWorked);
            rateNode.textContent = formatCurrency(result.hourlyRate);
            totalNode.textContent = formatCurrency(result.totalAmount);
            renderMessages(result.warnings, warningsList, 'text-warning');
            renderMessages(result.errors, errorsList, 'text-danger');
            setState(result.isValid ? 'valid' : 'invalid');
        }
        catch (error) {
            setState('error');
            errorsList.innerHTML = '<li>Automation service is unavailable. You can still submit the form.</li>';
            console.error(error);
        }
        finally {
            if (indicator) {
                indicator.textContent = '';
            }
        }
    };

    const debouncedUpdate = debounce(updateSummary, 250);

    hoursInput?.addEventListener('input', debouncedUpdate);
    rateInput?.addEventListener('input', debouncedUpdate);

    document.querySelectorAll('.quick-fill').forEach((button) => {
        button.addEventListener('click', () => {
            const targetId = button.dataset.target;
            const value = button.dataset.value;
            const input = document.getElementById(targetId);
            if (!input) return;
            input.value = value;
            input.dispatchEvent(new Event('input', { bubbles: true }));
            input.focus();
        });
    });

    debouncedUpdate();
})();
