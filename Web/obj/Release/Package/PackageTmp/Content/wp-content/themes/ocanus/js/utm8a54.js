(function() {
    // Extract parameters from the current URL
    const urlParams = new URLSearchParams(window.location.search);
    const utmParams = ['utm_source', 'utm_campaign', 'utm_location', 'utm_adgroup'];

    // If UTM parameters exist in the URL, save/overwrite them in sessionStorage
    utmParams.forEach(param => {
        if (urlParams.has(param)) {
            sessionStorage.setItem(param, urlParams.get(param));
        }
    });

    // Inject values into the hidden form inputs
    document.addEventListener("DOMContentLoaded", function() {
        utmParams.forEach(param => {
            const inputElement = document.getElementById(param);
            if (inputElement) {
                // Retrieve the value from sessionStorage
                const savedValue = sessionStorage.getItem(param);
                if (savedValue) {
                    inputElement.value = savedValue;
                }
            }
        });
    });
})();