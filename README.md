# 🎓 AituConnect

**AituConnect** is a Telegram bot designed to enhance the university experience by connecting students through shared interests, events, and academic pursuits. Whether you're looking to join clubs, attend events, or find study partners, AituConnect simplifies the process within your university community.([GitHub][1])

---

# Wiki
https://deepwiki.com/Vertiigor/NotificationSystem

---
## 🚀 Features

* **User Registration**: Sign up with your university and major information.
* **Post Creation**: Share posts categorized by subjects to find like-minded peers.
* **Post Browsing**: Explore posts from other students at your university.
* **Profile Management**: Update your personal information and academic details.
* **Notifications**: Stay informed about new posts and events relevant to your interests.

---

## 🛠️ System Architecture

AituConnect employs a distributed, message-driven architecture comprising two primary microservices:

1. **MessageListenerService**:

   * Handles incoming messages and commands from Telegram users.
   * Manages user sessions using Redis.
   * Routes messages to appropriate handlers based on the user's current interaction step.
   * Publishes messages to RabbitMQ queues for further processing.

2. **MessageProducerService**:

   * Consumes messages from RabbitMQ queues.
   * Executes business logic and performs database operations.
   * Sends responses back to users via the Telegram Bot API.

Communication between services is facilitated through RabbitMQ, ensuring scalability and reliability.

---

## 📚 User Interaction Pipelines

AituConnect utilizes a pipeline pattern to manage multi-step user interactions. Each pipeline represents a specific user journey, maintaining the user's current state throughout the process.

**Key Pipelines**:

* **Registration**:

  * Steps: `ChoosingMajor` → `ChoosingUniversity`
* **Post Creation**:

  * Steps: `TypingTitle` → `TypingContent` → `ChoosingSubject`
* **Profile Editing**:

  * Steps: `EditInputOption` → `EditMajor` → `EditUniversity`
* **Post Deletion**:

  * Steps: `ChoosingPost`
* **Post Listing**:

  * Steps: N/A (single-step interaction)

---

## 🗃️ Data Model

The core entities in AituConnect's data model include:

* **User**:

  * Attributes: `Id`, `Username`, `University`, `Major`
* **Post**:

  * Attributes: `Id`, `Title`, `Content`, `CreatedAt`, `UserId`
* **Subject**:

  * Attributes: `Id`, `Name`

Posts are associated with subjects through a many-to-many relationship, enabling users to tag posts with relevant academic subjects.

---

## ⚙️ Technologies Used

* **Programming Language**: C#
* **Framework**: ASP.NET Core
* **Database**: PostgreSQL
* **ORM**: Entity Framework Core
* **Caching**: Redis
* **Message Broker**: RabbitMQ
* **Containerization**: Docker
* **Bot Integration**: Telegram Bot API([GitHub][2])

---

## 📦 Getting Started

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/Vertiigor/AituConnect.git
   ```

2. **Build and Run the Services**:

   Use Docker Compose to build and run the services:

   ```bash
   docker-compose up --build
   ```

3. **Interact with the Bot**:

   Start a conversation with your Telegram bot and explore the features!

---

## 🤝 Contributing

Contributions are welcome! If you'd like to improve AituConnect, please fork the repository and submit a pull request. For major changes, open an issue first to discuss your proposed modifications.


*Empowering students to connect, collaborate, and thrive within their university communities.*([GitHub][1])
