let globals = {
    currentStock: null,
    token: null,
    inStock: null,
}

function detailModal(e) {
    globals.inStock = globals.currentStock.querySelectorAll("td")[2].innerText;
    if (globals.inStock <= 0) {
        document.querySelector(".sell-item").classList.add("invalid");
    }
    else {
        document.querySelector(".sell-item").classList.remove("invalid");
    }
    let fields = Array.from(globals.currentStock.querySelectorAll("td")).map(n => n.innerText);
    document.querySelector(".modal-item-id").innerText = fields[0];
    document.querySelector(".modal-item-name").innerText = fields[1];
    document.querySelector(".modal-item-qty").innerText = fields[2]
    document.querySelector(".detail-item-modal").classList.add("active");
    document.querySelector(".order-more").href = `/Order/CreateQuick/${globals.currentStock.id}`;
    let orderList = document.querySelector(".pending-orders");
    orderList.innerHTML = '';
    for (let order of ORDERS.filter(o => o.itemName == fields[1]))
    {
        let li = document.createElement("li");
        li.innerHTML = `
            <a href="/Order/${order.id}">Order #: ${order.id}</a>
        `
        orderList.appendChild(li);
    }
}

function newItemModal(e) {
    document.querySelector(".new-item-modal").classList.add("active");
}


function closeModal(e) {
    if (!e.target.classList.contains("close-modal") && 
        !e.target.classList.contains("item-modal") &&
        !e.target.classList.contains("cancel")) {
            return;
        }
    if (e.target.classList.contains("cancel-sale") && QUICK_SALE) {
        QUICK_SALE = 0;
        closeAllModals();
    }
    else {
        e.target.closest(".item-modal").classList.remove("active");
    }
}

function editItemModal(e) {
    closeModal(e);
    let itemName = globals.currentStock.querySelectorAll("td")[1].innerText;
    document.querySelector(".modal-item-name-edit").value = itemName;
    document.querySelector(".edit-item-modal").classList.add("active");
}

function editItem(e) {
    let itemName = document.querySelector(".modal-item-name-edit").value;
    fetch(`Stock/Edit/${globals.currentStock.id}/${itemName}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token
        }
    }).then((res) => {
        window.location.href="/Stock";
    })
}

function deleteItem(e) {
    fetch(`Stock/Delete/${globals.currentStock.id}`,
    {
        headers: {
            "RequestVerificationToken": globals.token,
        },
        method: "POST",
    }).then((res) => {
        window.location.href="/Stock";
    })
}

function sellItemModal() {
    console.log(globals.currentStock)
    if (globals.inStock == 0) {
        return;
    }
    document.getElementById("item-amount").max = globals.currentStock.lastElementChild.innerText;
    document.getElementById("item-name").value = globals.currentStock.querySelectorAll("td")[1].innerText;
    document.querySelector(".sell-item-modal").classList.add("active");
}

function closeAllModals() {
    document.querySelectorAll(".item-modal").forEach((elem) => {
        elem.classList.remove("active");
    })
}

function sellItem(e) {
    if (globals.inStock == 0) {
        return;
    }
    let id = globals.currentStock.id;
    let amount = document.querySelector("[name='sellAmount']").value;
    console.log(amount)
    if (!amount || amount > parseInt(globals.currentStock.lastElementChild.innerText)) {
        console.log("foo")
        return;
    }
    fetch(`/Stock/Sell/${id}/${amount}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token,
        }
    }).then((res) => {
        window.location.href="/Stock";
    })
}

window.addEventListener("load", (e) => {
    //Any stock with a count of 0 shouldn't be available to select in modal
    // Find all where count == 0 and remove <option> from DOM
    // Slice because first tr is header
    let stocks = Array.from(document.querySelectorAll("tr")).slice(1)
    zero_stocks = stocks.filter(s => s.lastElementChild.innerText == 0 )
    zero_stocks.forEach((s) => {
        // if value in <option> matches itemName of stock, remove it
        let itemName = s.querySelectorAll("td")[1].innerText;
        let selectOption = 
            document.querySelector(`option[value="${itemName}"]`);
        if (selectOption) {
            selectOption.remove();
        }
    })
    // We could get to this page from the quick action button on dashboard
    // If so, (check QUICK_SALE bool), we'll manually assign a currentStock
    // to globals, to let sell-item-modal populate correctly
    if (QUICK_SALE) {
        //find first stock that has count>0
        let currentStock = stocks.filter((s) => {
            return s.lastElementChild.innerText > 0;
            })[0];
        globals.currentStock = currentStock;
        sellItemModal();
    }
    
    document.querySelectorAll(".nav-btn").forEach((elem) => {
        elem.classList.remove("selected");
    });
    document.querySelector(".nav-stock").classList.add("selected");
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value
    document.querySelectorAll(".stock").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            globals.currentStock = e.target.closest("tr");
            detailModal()
        });
    })
    document.querySelectorAll(".close-modal, .item-modal").forEach((elem) => {
        elem.addEventListener("click", closeModal);
    });
    document.querySelector(".new-stock").addEventListener("click", newItemModal);
    document.querySelector(".edit").addEventListener("click", editItemModal);
    document.querySelectorAll(".cancel").forEach((elem) => {
        elem.addEventListener("click", closeModal)
    });
    document.querySelector(".delete").addEventListener("click", deleteItem);
    document.querySelector(".sell-item").addEventListener("click", sellItemModal);
    document.querySelector(".sell-item-btn").addEventListener("click", sellItem);
    document.querySelector(".save-item-btn").addEventListener("click", editItem);
});