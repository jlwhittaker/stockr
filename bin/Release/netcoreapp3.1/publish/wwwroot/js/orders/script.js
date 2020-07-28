let globals = {
    currentOrder: null,
    token: null, // initialized on page load
}

function closeModal() {
    // Virtual keyboard on mobile screws up our page, we hide/show footer if modal is open
    document.querySelector("footer").classList.add("active");

    document.querySelectorAll(".modal").forEach((elem) => {
    elem.classList.remove("active");
    });
}

function newOrderModal(e) {
    // Virtual keyboard on mobile screws up our page, we hide/show footer if modal is open
    document.querySelector("footer").classList.remove("active");
    document.querySelector(".new-order").classList.toggle("active");
}

function orderModal() {
    document.querySelector("footer").classList.remove("active");
    // Pull out values from <tr> to populate modal
    let fields = Array.from(globals.currentOrder.children).map(n => n.innerText);
    document.querySelector(".modal-order-id").innerText = fields[0];
    document.querySelector(".modal-item-name").innerText = fields[1];
    document.querySelector(".modal-item-qty").innerText = fields[2];
    document.querySelector(".modal-date").innerText = fields[3];
    document.querySelector(".modal-order-status").text = fields[4];
    document.querySelector(".order-modal").classList.add("active");
}

function updateStatus(e) {
    // split <select> into array to get correct value
    let newStatus = e.target.innerText.split('\n')[e.target.value];
    let statusID = e.target.value;
    let qty = globals.currentOrder.querySelector(".qty").innerText;
    fetch(`/Order/Update/${globals.currentOrder.id}/${statusID}/${qty}`, 
        { 
            method: "POST",
            headers: {
                "RequestVerificationToken": globals.token,
            }
        }
    ).then((response) => {
        if (response.redirected) {
            // Middleware gave us the boot
            window.location.href = response.url;
        }
        else {
            globals.currentOrder.querySelector("td:last-child").innerText = newStatus;
            closeModal();
        }
    });
}

// Hide/Show completed orders, attached to checkbox in footer
function showCompletedOrders(showOrders) {
    let orders = Array.from(document.querySelectorAll("tr")).slice(1);
    orders.filter((o) => {
        return o.querySelectorAll("td")[4].innerText == "Completed"
    }).forEach((o) => {
        if (showOrders) {
            o.classList.remove("hidden");
        } else {
            o.classList.add("hidden");
        }
    });
}

function deleteOrder(e) {
    fetch(`/Order/Delete/${globals.currentOrder.id}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token
        }
    }).then((res) => {
        window.location.href="/Order";
    });
}

window.addEventListener("load", (e) => {

    // Check for globals to see if we're doing a quick order, or going straight to a specific order
    if (ITEM_NAME) {
        // New Order modal visibility is toggled in server rendering, but we need to
        // make sure correct <select> option is picked
        document.querySelector("[name='itemName']").value = ITEM_NAME;
    }

    // Make sure correct nav button is darkened
    document.querySelector(".selected").classList.remove("selected");
    document.querySelector(".nav-orders").classList.add("selected");

    // get anti forgery token
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value;

    // I added click listeners to each td instead of tr
    // I don't know why, but I probably had a reason so I'm leaving it
    document.querySelectorAll("td").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            globals.currentOrder = e.target.closest("tr"); 
            orderModal();
        });
    });

    if (ORDER_ID) {
        // We're going straight to specific order, just click it
        document.getElementById(ORDER_ID).firstElementChild.click();
    }

    // Modal closing
    document.querySelectorAll(".close-modal, .cancel, .order-modal").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            if (!e.target.classList.contains("close-modal") &&
                !e.target.classList.contains("cancel") &&
                !e.target.classList.contains("order-modal")) {
                return;
            }
            closeModal();
    });

    //Various event listeners
    document.querySelector(".new-order-btn").addEventListener("click", newOrderModal);
    document.querySelector("select[name=status]").addEventListener("change", updateStatus);
    document.querySelector(".delete").addEventListener("click", deleteOrder);
    document.getElementById("showCompleted").addEventListener("change", (e) => {
        showCompletedOrders(e.target.checked);
        });
    });
});
