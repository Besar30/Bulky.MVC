let CartCount = document.getElementById("CartCount");

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/cart")
    .build();

function success() {
    connection.invoke("GetCartCount").then((value) => {
        CartCount.innerText = value.toString();
    });
}
connection.start()
    .then(success)
    .catch();