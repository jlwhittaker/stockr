function dismissAlert(id) {
    let token = document.querySelector("[name=__RequestVerificationToken]").value;
    fetch(`StockAlert/Dismiss/${id}`,
        {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            }
        }
    ).then((data) => {
        return data.json();
    }).then((res) => {
        if (res.success) {
            let alert = document.getElementById(`alert-${id}`);
            alert.remove();
            let alertCount = document.getElementById("alert-count");
            alertCount.innerText = (parseInt(alertCount.innerText)-1).toString();
        }
    });
}
