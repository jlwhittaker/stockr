// The handler is attached inline in the markup, here's the definition

function dismissAlert(id) {
    let token = document.querySelector("[name=__RequestVerificationToken]").value;
    fetch(`StockAlert/Dismiss/${id}`,
        {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            }
        }
    ).then((response) => {
        if (response.redirected) {
            // Middleware for auth probably booted us out
            window.location.href = response.url;
        }
        else {
            return response.json();
        }
    }).then((res) => {
        if (res.success) {
            let alert = document.getElementById(`alert-${id}`);
            alert.remove();
            let alertCount = document.getElementById("alert-count");
            alertCount.innerText = (parseInt(alertCount.innerText)-1).toString();
        }
    });
}
