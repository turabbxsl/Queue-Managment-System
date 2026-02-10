# Queue Management System

A real-time Queue Management System built with **.NET 10** and **Clean Architecture**, featuring a **SignalR-powered UI** for live ticket flow, automatic desk assignment, VIP prioritization and PostgreSQL persistence.

The system simulates a real-world bank/service queue where customers are automatically routed to the most suitable desk.

---

## Features

- Real-time UI using **SignalR**
- Live ticket updates without page refresh
- Automatic desk selection (free desk or shortest queue)
- VIP customer prioritization
- Desk-based queue management
- Linked List based queue logic (persisted in database)
- Fully asynchronous API
- Clean Architecture (API / Application / Core / Infrastructure)
- Generic repository pattern for future scalability

---

## Tech Stack

- **.NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **SignalR**
- **Clean Architecture**

---

## Queue Logic (Linked List)

The system implements a **Linked List queue structure** at the database level.

Each ticket contains:
- `NextTicketId`
- `PreviousTicketId`

Each desk maintains:
- `HeadTicketId`
- `TailTicketId`
- `QueueCount`

This allows:
- O(1) enqueue and dequeue operations
- Correct ordering even after restarts
- Persistent queue state across system reboots

---

## Desk Assignment Strategy

When a new ticket is created:

1. If there is a **free desk**, the ticket is assigned immediately.
2. Otherwise, the ticket is assigned to the desk with the **shortest queue**.
3. VIP customers are placed **ahead of regular customers** in the queue.

This logic mirrors real-world bank/service reception behavior.

---

## Real-Time UI with SignalR

SignalR is actively used on the **UI layer**, not only on the backend.

The frontend listens to live events such as:
- New ticket created
- Ticket assigned to desk
- Ticket moved between desks
- Ticket status updated

This enables:
- No page refresh
- Real-time operator screens
- Instant queue updates for all connected clients

---

## Architecture

The solution follows **Clean Architecture** principles:

