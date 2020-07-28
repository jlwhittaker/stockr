let globals = {
    currentOrder: null,
    token: null,
}

function closeModal() {    
    document.querySelector(".modal").classList.remove("active");
}

function newOrderModal(e) {
    document.querySelector(".new-order").classList.toggle("active");
}

function orderModal() {
    let fields = Array.from(globals.currentOrder.children).map(n => n.innerText);
    console.log(fields)
    document.querySelector(".modal-order-id").innerText = fields[0];
    document.querySelector(".modal-item-name").innerText = fields[1];
    document.querySelector(".modal-item-qty").innerText = fields[2];
    document.querySelector(".modal-date").innerText = fields[3];
    console.log(fields[4])
    document.querySelector(".modal-order-status").text = fields[4];
    document.querySelector(".order-modal").classList.add("active");
}

function updateStatus(e) {
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
    ).then((data) => {
        return data.json();
    }).then((res) => {
        if (res.success) {
            globals.currentOrder.querySelector("td:last-child").innerText = newStatus;
        }
        closeModal();
    })
}

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
    }).then((data) => {
        window.location.href="/Order";
    });
}

window.addEventListener("load", (e) => {
    if (ITEM_NAME) {
        document.querySelector("[name='itemName']").value = ITEM_NAME;
    }
    document.querySelector(".selected").classList.remove("selected");
    document.querySelector(".nav-orders").classList.add("selected")
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value
    document.querySelectorAll("td").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            globals.currentOrder = e.target.closest("tr"); 
            orderModal();
        });
    });
    if (ORDER_ID) {
        document.getElementById(ORDER_ID).firstElementChild.click();
    }

    document.querySelectorAll(".close-modal, .cancel, .order-modal").forEach((elem) => {
        
        elem.addEventListener("click", (e) => {
            if (!e.target.classList.contains("close-modal") &&
                !e.target.classList.contains("cancel") &&
                !e.target.classList.contains("order-modal")) {
                return;
            }
            closeModal();
    });
    document.querySelector(".new-order-btn").addEventListener("click", newOrderModal);
    document.querySelector("select[name=status]").addEventListener("change", updateStatus);
    document.querySelector(".delete").addEventListener("click", deleteOrder);
    document.getElementById("showCompleted").addEventListener("change", (e) => {
        showCompletedOrders(e.target.checked);
        });
    });
});
