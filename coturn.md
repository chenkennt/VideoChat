# Setup TURN server

1. Install coturn package

   ```
   sudo apt-get install coturn
   ```

2. Enable coturn

   ```
   vi /etc/default/coturn
   ```

   Uncomment the following:
   ```
   TURNSERVER_ENABLED=1
   ```

3. Update config

   ```
   vi /etc/turnserver.conf
   ```

   Uncomment the following
   ```
   listening-ports=3478
   fingerprint
   lt-cred-mech
   realm=<your_realm>
   ```

4. Restart coturn

   ```
   systemctl restart coturn
   ```

5. Add user

   Generate key 
   ```
   turnadmin -k -u <username> -r <your_realm> -p <password>
   ```

   Add user
   ```
   turnadmin -a -u <username> -r <your_realm> -p <key>
   ```

6. When you create an `RTCPeerConnection`, add TURN server:

   ```js
   var peer = new RTCPeerConnection({
     [{
       urls: "turn:<your server>:3478",
       username: "<username>",
       credential: "<key>"
     }]
   });
   ```