let globals = {
    currentAlert: null,
    token: null,
}

function search(e) {

}

function detailModal(e) {
    if (e.target.classList.contains("alert-on") || 
        e.target.classList.contains("alert-on-label")) 
    {
        updateAlert(e);
        return;
    }
    let fields = Array.from(globals.currentAlert.querySelectorAll("td"));
    document.querySelector(".modal-alert-id").innerText = fields[0].innerText
    document.querySelector(".modal-alert-name").innerText = fields[1].innerText;
    let dropDown = document.querySelector(".modal-alert-type");
    dropDown.selectedIndex = Array.from(dropDown.options).findIndex((opt) => {
        return (opt.text === fields[2].innerText);
    });
    if (dropDown.value == 0) {
        document.querySelector(".threshold-wrapper").classList.add("active");
    }
    else {
        document.querySelector(".threshold-wrapper").classList.remove("active");
    }
    document.querySelector(".modal-alert-threshold").value = fields[3].innerText;
    document.getElementById("alert-active").checked = fields[4].querySelector("input").checked
    document.querySelector(".detail-modal").classList.add("active");
}

function newAlertModal(e) {
    document.querySelector(".new-alert-modal").classList.add("active");
}

// 'Save' link in modal is clicked, populate form data object and POST
function saveAlert(e) {
    let formData = new FormData();
    let id = globals.currentAlert.id;
    let alertType = document.querySelector(".modal-alert-type");
    let threshold = document.querySelector(".modal-alert-threshold").value || 0;
    formData.append("alertType", alertType);
    formData.append("lowStockThreshold", threshold);
    let url = `/StockAlert/Edit/${id}/${alertType.value}/${threshold}`;
    fetch(url, 
        {
            method: "POST",
            headers: {
                "RequestVerificationToken": globals.token,
            },
            redirect: "follow",
        }).then((res => {
            window.location.href="/StockAlert";
        }))
}

// Check box in list view is checked
function updateAlert(e) {
    if (!document.readState === "complete") 
    {
        e.stopPropagation();
        e.preventDefault();
        return;
    }
    if (e.target.classList.contains("alert-on-label")) {
        return;
    }
    e.target.classList.remove("active");
    e.target.closest("div").querySelector(".alert-on-loading").classList.add("active");
    let s = e.target.checked ? "On" : "Off";
    fetch(`/StockAlert/${s}/${globals.currentAlert.id}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token,
        }
    }).then(d => d.json()).then((res) => {
        if (res.success) {
            globals.currentAlert.querySelector("input").checked = e.target.checked;
            e.target.classList.add("active");
            e.target.closest("div").querySelector(".alert-on-loading").classList.remove("active");
        }
    })
}

function closeModal(e) {
    if (!e.target.classList.contains("modal") && 
        !e.target.classList.contains("close-modal") &&
        !e.target.classList.contains("delete")) {
        return;
    }
    e.target.closest(".modal").classList.remove("active");
}

function deleteAlert(e) {
    fetch(`/StockAlert/Delete/${globals.currentAlert.id}`, 
        {
            method: "POST",
            headers: {
                "RequestVerificationToken": globals.token,
            }
        }).then((res) => {
            window.location.reload();
        })
}

window.addEventListener("load", () => {
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value;

    document.querySelectorAll(".nav-btn").forEach((btn) => {
        btn.classList.remove("selected");
    })
    document.querySelector(".nav-alerts").classList.add("selected")
    // Open Detail Modal
    document.querySelectorAll(".alert").forEach((alert) => {
        alert.addEventListener("click", (e) => {
            globals.currentAlert = e.target.closest("tr");
            detailModal(e);
        });
    });

    // Close Modal
    document.querySelectorAll(".modal, .close-modal, .delete").forEach((elem) => {
        elem.addEventListener("click", closeModal);
    });

    // Delete alert
    document.querySelector(".delete-alert").addEventListener("click", deleteAlert);

    // Alert-On checkbox is checked
    document.querySelectorAll(".alert-on.alert-on-modal").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            updateAlert(e);
        });
    })

    //Save link in modal is clicked, save alert
    document.querySelector(".save-alert").addEventListener("click", saveAlert);

    // New alert
    document.querySelector(".new-alert").addEventListener("click", newAlertModal);

    // Show threshold input depending on alert type
    document.querySelectorAll(".modal-alert-type").forEach((elem) => {
        elem.addEventListener("change", (e) => {
        if (e.target.value == 0) {
            document.querySelectorAll(".threshold-wrapper").forEach((elem) => {
                elem.classList.add("active");
            });
        }
        else {
            document.querySelectorAll(".threshold-wrapper").forEach((elem) => {
                elem.classList.remove("active");
            });
        }
        });
    });
});