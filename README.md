<h3>Stockr</h3>
        <h4>Here's how it works:</h4>
        <p>Your inventory consists of items, called 'stocks'. Once you have items in stock,
          there's a few things you can do:</p>
          <ul>
            <li>Place an order for more</li>
            <li>Update an order status</li>
            <li>Sell some of the stock</li>
            <li>Set up an alert</li>
          </ul>
          <p>Updating an order status to 'Completed' will automatically update
            the quantity that you have in stock. Same goes for selling an item.
          </p>
          <p>There are two types of alerts -- low stock, and out-of-stock.
            When an alert gets triggered, you'll be able to see it on the dashboard. Clicking the
            checkbox next to the alert notification will dismiss the alert. Any change to the item quantity 
            (whether via an order or sale), will re-trigger any relevant alerts. Alerts can be turned on and 
            off on the Alert page.
          </p>
          <p>Feel free to create your own user, but please don't use any credentials that you use for 
            something else. The default login is:
          </p>
          <p class="credentials">Username: <span>"test"</span></p>
          <p class="credentials">Password: <span>"pw"</span></p>
          <p>Note: This page is open to the public, so it's possible that some unscrupulous
            users may have added some ... questionable content to the default account. The database gets reset every day, though.
          </p>
          <div class="modal-button">
            <a href="http://stockr.jonlwhittaker.com" target="_blank">Try it out</a>
          </div>
          <h4>Compiling yourself</h4>
          <p>The compilation process is handled by the Docker container. The HTTP server functionality is handled by Kestrel, but all of that is abstracted away for the most part. All you really need to do is build and run the container, choosing which port to expose. From inside the directory with the Dockerfile, you can run: <br/><br/><code>docker build -t stockr .</code><br/><br/>
Now that the container is built, run it using <br/><br/><code>docker run -p 8888:80 -d stockr</code><br/><br/>
to use port 8888 on the host machine.</p>


