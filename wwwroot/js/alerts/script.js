let globals = {
    currentAlert: null,
    token: null, // Get's assigned on window load
}

function detailModal(e) {

    // Get values from tr element to populate detail modal
    let fields = Array.from(globals.currentAlert.querySelectorAll("td"));

    document.querySelector(".modal-alert-id").innerText = fields[0].innerText
    document.querySelector(".modal-alert-name").innerText = fields[1].innerText;
    let dropDown = document.querySelector(".modal-alert-type");
    dropDown.selectedIndex = Array.from(dropDown.options).findIndex((opt) => {
        return (opt.text === fields[2].innerText);
    });
    // Only show threshold input if alert type is lowStock
    if (dropDown.value == 0) {
        document.querySelector(".threshold-wrapper").classList.add("active");
    }
    else {
        document.querySelector(".threshold-wrapper").classList.remove("active");
    }
    document.querySelector(".modal-alert-threshold").value = fields[3].innerText;
    document.getElementById("alert-active").checked = fields[4].querySelector("input").checked;

    // Modal is populated, render to DOM
    document.querySelector(".detail-modal").classList.add("active");
}

function newAlertModal(e) {
    document.querySelector(".new-alert-modal").classList.add("active");
}

// 'Save' link in modal is clicked, send model params via route params
function saveAlert(e) {
    let id = globals.currentAlert.id;
    let alertType = document.querySelector(".modal-alert-type");
    let threshold = document.querySelector(".modal-alert-threshold").value || 0;

    let url = `/StockAlert/Edit/${id}/${alertType.value}/${threshold}`;

    fetch(url, 
        {
            method: "POST",
            headers: {
                "RequestVerificationToken": globals.token,
            },
            credentials: 'include',
        }).then((res) => {
            if (res.redirected) {
                // Middleware gave us the boot
                window.location.href = res.url;
            }
            window.location.href="/StockAlert";
        });
}

// Check box in list view/modal is checked
function updateAlert(e) {
    // Don't do anything if page isn't loaded fully
    if (!document.readyState === "complete") 
    {
        e.stopPropagation();
        e.preventDefault();
        return;
    }

    // The label element here is actually the checkbox itself, with a check <img> inside it
    let input = e.target.closest("div").querySelector("input");
    let checkbox = e.target.closest("label");
    // Hide checkbox to show loading icon
    checkbox.classList.remove("active");
    checkbox.closest("div").querySelector(".alert-on-loading").classList.add("active");
    // Send POST to update alert
    let s = input.checked ? "Off" : "On";
    fetch(`/StockAlert/${s}/${globals.currentAlert.id}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token,
        },
        credentials: "include",

    }).then(res => {
        if (res.redirected) {
            // Middleware gave us the boot
            window.location.href = res.url;
        }
        else {
            // Manually 'check' hidden checkbox input on page
            // This is necessary because checkbox for alert status also exists in modal
            globals.currentAlert.querySelector("input").checked = input.checked;
            // Hide loading icon and show checkbox again
            e.target.closest("div").querySelector(".alert-on-loading").classList.remove("active");
            checkbox.classList.add("active");
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
            },
            credentials: 'include',
        }).then((res) => {
            if (res.redirected) {
                // Middleware gave us the boot
                window.location.href = res.url;
            }
            window.location.href="/StockAlert";
        })
}

window.addEventListener("load", () => {
    // Get anti-forgery token 
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value;

    // Make sure proper nav button is darkened
    document.querySelectorAll(".nav-btn").forEach((btn) => {
        btn.classList.remove("selected");
    })
    document.querySelector(".nav-alerts").classList.add("selected")

    // Open Detail Modal
    document.querySelectorAll(".alert td:not(:last-child)").forEach((alert) => {
        alert.addEventListener("click", (e) => {
            // Set currentAlert any time a tr is clicked
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
    document.querySelectorAll(".custom-checkbox-main").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            // currentAlert needs to be set if clicked from list view
            globals.currentAlert = e.target.closest("tr");
            updateAlert(e);
        });
    })
    document.querySelectorAll(".custom-checkbox-modal").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            updateAlert(e);
        });
    })

    //Save alert
    document.querySelector(".save-alert").addEventListener("click", saveAlert);

    // New alert
    document.querySelector(".new-alert").addEventListener("click", newAlertModal);

    // hide/Show threshold input if alertType <select> changes
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