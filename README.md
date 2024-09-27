# **Umbrella - Night-out Party Tracker**

## **Project Overview**

**Umbrella** is a web application designed to help organizers manage group events and track the proximity of participants during the event. The app focuses on ease of use, privacy, and real-time updates. The organizer can create events, invite participants, and monitor if any attendees leave the predefined event radius. Privacy and control are emphasized, as users can choose to disable tracking or exit events early.

---

## **Project Features**

### **Organizer Features**
1. **Create Group and Event Link**
   - The organizer creates a group for the event, generating a unique URL that attendees use to join.
   - Each group is associated with specific event details, such as time, location, and participants.
   
2. **Set Event Time and Location**
   - Organizers define the event’s start time, duration, and meeting place.
   - A **Google Maps** interface allows the organizer to set the meeting point, which is used for proximity tracking.

3. **Define Proximity Radius**
   - The organizer can specify a proximity radius around the meeting place.
   - If any participant leaves this radius, the organizer is notified.

4. **Real-Time Notifications**
   - Organizers receive real-time notifications when attendees leave the radius.
   - Attendees can also choose to notify the organizer if they are leaving early, maintaining clear communication.

5. **Invite Management**
   - The organizer can send invitation links to attendees, allowing them to join the group.
   - Invitations can collect additional information such as phone numbers.
   - Links are time-limited and can be set to expire automatically.

6. **Event Modifications**
   - Organizers can add or remove participants and modify event details, including the meeting point.
   - They can also control which attendees can invite others.

7. **Contact Attendees**
   - The organizer can call or transfer calls to the phone app for easy communication with attendees.
   
8. **Automatic Group Expiration**
   - After the event ends, the group data (including personal information and locations) is deleted automatically for privacy.

---

### **Attendee Features**
1. **Join Event via Invite Link**
   - Attendees can join events using a unique link shared by the organizer.
   - If required, they may provide their phone number upon joining.

2. **Real-Time Tracking (Optional)**
   - Attendees’ locations are monitored to ensure they remain within the defined proximity radius.
   - If a user leaves, they can send a notification to the organizer, letting them know they’ve left voluntarily.

3. **Disable Tracking**
   - Attendees have the option to disable tracking, giving them control over their privacy.

4. **Exit Event Early**
   - Attendees can leave the group early and can notify the organizer when they do so.
   - Depending on permissions, attendees may be allowed to invite others to the event.

---

## **High-Level System Architecture**

### **1. Backend: ASP.NET Core**
   - The server-side of the application will be built with **ASP.NET Core**, hosted in **Azure App Service** for scalability and ease of deployment.
   - All business logic, such as group creation, location tracking, and notifications, will be handled on the server.
   - **API Endpoints** will be created for interaction between the front-end and back-end.

### **2. Database: SQL/NoSQL**
   - **Azure SQL** or **Firebase Firestore** will be used for storing event data, group details, and user information.
   - Data encryption should be implemented to ensure that personal information is stored securely.
   - All user data should be deleted after the event ends to prioritize privacy.

### **3. Location Services: Google Maps API**
   - **Google Maps JavaScript API** will be used to allow organizers to set the event location and proximity radius.
   - The **Geolocation API** will be used to track participants’ real-time locations and calculate their distance from the meeting point using the **Haversine formula**.

### **4. Notifications: Firebase Cloud Messaging / Azure Notification Hubs**
   - Real-time notifications will be sent to the organizer when attendees leave the proximity radius or notify that they are leaving early.
   - **Firebase Cloud Messaging (FCM)** or **Azure Notification Hubs** can be used for push notifications.

### **5. Mobile Integration**
   - The app can be accessed via mobile browsers with **PWA (Progressive Web App)** support to make it feel like a native app.
   - Consider adding features for direct integration with the phone’s dialer app using `tel:` URLs.

### **6. Real-Time Communication: SignalR / Firebase**
   - Implement **SignalR** for real-time communication between the organizer and participants. It enables live updates on changes such as proximity alerts, group changes, etc.
   - Alternatively, **Firebase** can be used for real-time updates.

### **7. Privacy & Security**
   - **HTTPS** should be used to encrypt data in transit.
   - Personal data (e.g., phone numbers, locations) should be encrypted in the database and deleted after the event to comply with privacy standards.
   - Ensure users have control over their data with features like “disable tracking” and the ability to exit groups early.

---

## **Cloud Infrastructure & DevOps**

### **1. Azure Integration**
   - Use **Azure App Service** to host the ASP.NET Core backend.
   - Use **Azure Functions** or **Azure Logic Apps** for automating tasks such as event expiration and group data deletion.

### **2. Docker**
   - The application can be containerized using **Docker** for easier deployment across different environments.
   - Use **Azure Kubernetes Service (AKS)** for managing and scaling containers if the app grows in complexity.

### **3. Continuous Integration/Continuous Deployment (CI/CD)**
   - Set up CI/CD pipelines using **GitHub Actions** or **Azure DevOps** to automate the deployment process. This ensures smooth updates and integration of new features.
   - Use **Docker** images to ensure consistency between development, testing, and production environments.

---

## **Next Steps**
- Finalize the app’s feature set and prioritize based on development complexity.
- Begin by developing the core event creation and proximity tracking features.
- Integrate **Google Maps API** for location services.
- Explore **Azure** for hosting and explore **Docker** for containerization.

---

