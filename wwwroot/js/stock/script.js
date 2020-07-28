let globals = {
    currentStock: null,
    token: null, //initialized on page load
    inStock: null,
}

function detailModal(e) {
    // Gray out 'Sell' link if item is out of stock
    if (globals.inStock <= 0) {
        document.querySelector(".sell-item").classList.add("invalid");
    }
    else {
        document.querySelector(".sell-item").classList.remove("invalid");
    }

    // Pull out fields to populate modal
    let fields = Array.from(globals.currentStock.querySelectorAll("td")).map(n => n.innerText);
    document.querySelector(".modal-item-id").innerText = fields[0];
    document.querySelector(".modal-item-name").innerText = fields[1];
    document.querySelector(".modal-item-qty").innerText = fields[2]
    document.querySelector(".order-more").href = `/Order/CreateQuick/${globals.currentStock.id}`;

    //populate list of pending orders
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

    // render modal
    document.querySelector(".detail-item-modal").classList.add("active");

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
        // Modals are normally stacked up, but if we got here from quick-sale, close all
        QUICK_SALE = 0;
        closeAllModals();
    }
    else {
        e.target.closest(".item-modal").classList.remove("active");
    }
}

function editItemModal(e) {
    let itemName = globals.currentStock.querySelectorAll("td")[1].innerText;
    document.querySelector(".modal-item-name-edit").value = itemName;
    document.querySelector(".edit-item-modal").classList.add("active");
}

//Send new item name to server and reload page
function editItem(e) {
    let itemName = document.querySelector(".modal-item-name-edit").value;
    fetch(`Stock/Edit/${globals.currentStock.id}/${itemName}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token
        },
        credentials: 'include',
    }).then((res) => {
        if (res.redirected) {
            window.location.ref = res.url;
        }
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
        credentials: 'include',
    }).then((res) => {
        if (res.redirected) {
            window.location.ref = res.url;
        }
        window.location.href="/Stock";
    })
}

function sellItemModal() {
    // Don't do anything if item is out of stock
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
    // Don't do anything if item is out of stock
    if (globals.inStock == 0) {
        return;
    }
    let id = globals.currentStock.id;
    let amount = document.querySelector("[name='sellAmount']").value;
    // Do nothing if invalid amount, or if trying to sell more than there are
    if (!amount || amount > parseInt(globals.currentStock.lastElementChild.innerText)) {
        return;
    }
    fetch(`/Stock/Sell/${id}/${amount}`, {
        method: "POST",
        headers: {
            "RequestVerificationToken": globals.token,
        },
        credentials: 'include',
    }).then((res) => {
        if (res.redirected) {
            window.location.ref = res.url;
        }
        window.location.href="/Stock";
    })
}

window.addEventListener("load", (e) => {
    // Any stock with a count of 0 shouldn't be available to select in modal
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
    
    //Make sure correct nav button is darkened
    document.querySelectorAll(".nav-btn").forEach((elem) => {
        elem.classList.remove("selected");
    });
    document.querySelector(".nav-stock").classList.add("selected");

    // get anti forgery token
    globals.token = document.querySelector("[name=__RequestVerificationToken]").value

    //Listeners to open modal detail
    document.querySelectorAll(".stock").forEach((elem) => {
        elem.addEventListener("click", (e) => {
            globals.currentStock = e.target.closest("tr");
            globals.inStock = globals.currentStock.querySelectorAll("td")[2].innerText;
            detailModal()
        });
    })

    //Close modal
    document.querySelectorAll(".close-modal, .item-modal").forEach((elem) => {
        elem.addEventListener("click", closeModal);
    });

    //Various listeners
    document.querySelector(".new-stock").addEventListener("click", newItemModal);
    document.querySelector(".edit").addEventListener("click", editItemModal);
    document.querySelectorAll(".cancel").forEach((elem) => {
        elem.addEventListener("click", closeModal)
    });
    document.querySelector(".delete").addEventListener("click", deleteItem);
    document.querySelector(".sell-item").addEventListener("click", sellItemModal);
    document.querySelector(".sell-item-btn").addEventListener("click", sellItem);
    document.querySelector(".save-item-btn").addEventListener("click", editItem);

    //Change currentStock in global when <select> changes
    document.getElementById("item-name").addEventListener("change", (e) => {
        let newItemName = e.target.selectedOptions[0].text;
        let items = Array.from(document.querySelectorAll(".stock"));
        let currentItem = items.filter((item) => {
            return item.querySelector(".item-name").innerText == newItemName;
        })[0];
        globals.currentStock = currentItem;
        globals.inStock = parseInt(currentItem.querySelector("td:last-child").innerText);

    });
});