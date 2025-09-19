# Signature Restaurant

> A full‑stack ASP.NET MVC web application for restaurant operations: table management, menu/catalog, ordering, kitchen workflow, stock/ingredients, offers, feedback/reviews, and basic analytics.

---

## Table of Contents

* [Overview](#overview)
* [Core Features](#core-features)
  * [Admin](#admin)
  * [Staff](#staff)
  * [Chef (Kitchen)](#chef-kitchen)
  * [Customer](#customer)
  * [Visitor](#visitor)
* [Tech Stack](#tech-stack)
* [Architecture](#architecture)
* [Database Schema](#database-schema)
* [Screens & Flows](#screens--flows)
* [Getting Started](#getting-started)
  * [Prerequisites](#prerequisites)
    * [Database Setup](#database-setup)
    * [Run](#run)
* [Seed Data & Admin Access](#seed-data--admin-access)
* [Testing](#testing)
* [Reports & Analytics](#reports--analytics)
* [Limitations](#limitations)
* [Roadmap / Future Enhancements](#roadmap--future-enhancements)
* [Project Planning (Process Notes)](#project-planning-process-notes)
* [Acknowledgments](#acknowledgments)

---

## Overview

**Signature Restaurant** is a full‑stack, role‑based restaurant management system that streamlines front‑of‑house and back‑of‑house operations in one MVC web app. It replaces paper tickets and ad‑hoc spreadsheets with a centralized database, predictable workflows, and actionable reports.

**What problems it solves**

* **Order accuracy & handoff:** Structured order lifecycle (Pending → Preparing → Complete) reduces miscommunication between staff and kitchen.
* **Menu & pricing control:** Central management for categories, items, images, veg/non‑veg, spice level, and pricing.
* **Inventory awareness:** Item‑to‑ingredient mapping ensures stock visibility and prevents selling out‑of‑stock items.
* **Customer experience:** Online table booking with confirmation/pending states; OTP flows; item ratings and reviews inform the menu.
* **Promotion management:** Offers with date ranges and amount thresholds; optional bulk email campaigns.
* **Operational insight:** Daily/weekly/monthly sales, table utilization, ingredient consumption, and feedback trends.

**Primary users & responsibilities**

* **Admin:** System configuration, user/role governance, catalog, inventory, offers, reporting.
* **Staff (Front‑of‑house):** Take/modify orders, manage tables, view kitchen status, print invoices.
* **Chef (Kitchen):** Real‑time queue of items to prepare; update item/order status.
* **Customer/Visitor:** Browse menu, book tables, submit feedback/reviews.

**Non‑functional goals**

* **Reliability:** Clear validation rules (dates/numerics/text) and audit logs on status transitions.
* **Security:** Role‑based access; OTP flows for password reset; guidance to enable password hashing before production use.
* **Maintainability:** Conventional ASP.NET MVC structure (controllers, views, models), minimal external dependencies.
* **Extensibility:** Pluggable SMTP/SMS providers; adaptable DB engine (MySQL default; SQL Server possible).

**Deployment assumptions**

* Runs on IIS/IIS Express with MySQL 8.x (or SQL Server if adapted).
* SMTP/SMS configured for notifications (OTP, offers, booking confirmations).

> **Goal:** Deliver an end‑to‑end operational backbone for restaurants—reducing friction in ordering, increasing data quality, and enabling data‑driven decisions.

## Core Features

### Admin

* User management: create/edit/delete **Admin**, **Staff**, **Chef**, and **Customer** accounts.
* Table management: CRUD tables, capacity, and active state; view **Table Report**.
* Category & Item management: CRUD categories/items, including image, description, price, veg/non‑veg, spicy level.
* Order management: create/edit orders; view **Preparing**, **Completed** lists; generate **Invoice**.
* Offers management: CRUD offers (name, image, start/end date, total amount threshold, discount, active); bulk‑email customers when new offers are created.
* Stock & Ingredients: suppliers, stock intake, item‑ingredient mapping, stock reports, ingredient reports.
* Feedback/Reviews: view feedback and item ratings; basic sentiment review.
* Logs: view operational logs per order status transitions.
* Reporting: daily/weekly/monthly item sales; total/used/available stock.

### Staff

* View categories & items on tablet/terminal.
* Create orders, add items, start/continue order; warning when item stock is unavailable.

### Chef (Kitchen)

* Receives new orders in real time with customer notes/special requests.
* Updates item/order status (e.g., Pending → Preparing → Complete) and notifies staff/customer when ready.

### Customer

* Register/login, edit profile, change password, reset via OTP.
* Browse menu by category; rate items and leave reviews after sampling/ordering.
* Provide feedback about the restaurant.
* Online **Table Booking** with confirmation/pending status and SMS notification.

### Visitor

* Public pages: Home (offers, reviews), Menu, Contact, Book Table.

## Tech Stack

* **Frontend:** ASP.NET MVC (Razor Views), HTML, CSS/SCSS, JavaScript/jQuery.
* **Backend:** C# (ASP.NET MVC 5).
* **Database:** MySQL (SQL script provided).

  > *Note:* Some documentation references SQL Server; the codebase and script (`SQLQuery1.sql`) target **MySQL**. If you prefer SQL Server, adapt the schema and connection string accordingly.
* **Server:** IIS/IIS Express.
* **Auth & Roles:** Application‑level roles (Admin/Staff/Chef/Customer/Visitor).
* **Notifications:** Email (offers, OTP) and SMS (OTP/table booking); pluggable via SMTP/SMS gateway.

## Architecture

* **Pattern:** ASP.NET MVC (Models / Views / Controllers).
* **Layers:**

  * **Presentation:** Razor views for Admin/Staff/Chef/Customer/Visitor.
  * **Business Logic:** Controllers + Services for order state, stock checks, offers rules.
  * **Data Access:** Repository/ORM or ADO.NET (project uses SQL scripts; review `DAL`/`Context` in solution when opening the code).
* **Key Flows:**

  * **Ordering:** Staff creates order → Kitchen prepares → Completion → Invoice.
  * **Stock:** Admin intakes stock → Item–Ingredient usage decrements stock → Reports.
  * **Offers:** Admin creates offer → Email campaign → Checkout applies discount thresholds.
  * **Feedback/Reviews:** Customers submit → Admin dashboard aggregates ratings.

## Database Schema

Tables (core columns abbreviated):

* `Tbl_user` (UserID PK, Name, Contact, Email, Username, Password, IsBlock, DateTime, UserType)
* `Tbl_table` (TableID PK, TableNumber, Capacity, Active)
* `Tbl_category` (CategoryID PK, Name)
* `Tbl_item` (ItemID PK, CategoryID FK, Name, Description, Image, Type, SpicyLevel, Price)
* `Tbl_order` (OrderID PK, TableID FK, UserID FK, NoOfPerson, DateTime, Status, Discount, TotalAmount, Contact, OrderType)
* `Tbl_orderitem` (OrderItemID PK, OrderID FK, ItemID FK, Qty, Price, Status)
* `Tbl_review` (ReviewID PK, OrderID FK, Name, Contact, Rating, Review)
* `Tbl_log` (LogID PK, OrderID FK, Status, DateTime)
* `Tbl_supplier` (SupplierID PK, Name, Contact, Email, Address, Landmark, Pincode)
* `Tbl_ingredients` (IngredientsID PK, Name, Image)
* `Tbl_stock` (StockID PK, SupplierID FK, IngredientsID FK, Qty)
* `Tbl_itemingredients` (ItemIngID PK, ItemID FK, IngredientsID FK, Qty)
* `Tbl_offer` (OfferID PK, Name, Image, StartDate, EndDate, TotalAmount, Discount, Active)
* `Tbl_feedback` (FeedbackID PK, CustomerID FK, Feedback)
* `Tbl_customer` (CustomerID PK, Name, ContactNumber, EmailID, Address, Pincode)
* `Tbl_booktable` (BookTableID PK, Name, Guest, Date, Time, Contact, Status)

> The repo includes an SQL script (`SQLQuery1.sql`) you can use to create and seed schema/data.

## Screens & Flows

* **Auth:** Login, Forgot Password (OTP via email for Admin; via SMS for Staff/Chef/Customer).
* **Dashboards:** Admin (global KPIs); Staff (order & item focus); Chef (kitchen queue & item state).
* **CRUD Screens:** Users, Tables, Categories, Items, Suppliers, Stock, Ingredients, Offers.
* **Order Management:** Add order, order list, order item details, prepare/complete queues, invoice generation.
* **Customer‑Facing:** Home (offers & reviews), Menu (by category), Feedback, Contact Us, Online Table Booking.
* **Reports:** Tables, Items, Stock (total/used/available), Ingredients, Sales (daily/weekly/monthly).

## Getting Started

### Prerequisites

* **OS:** Windows 10 or later
* **IDE:** Visual Studio 2019 or 2022 (ASP.NET MVC workload)
* **.NET:** ASP.NET MVC 5 / .NET Framework 4.7.2+
* **Database:** MySQL 8.x
* **Driver:** MySQL Connector/NET
* **Server:** IIS Express (bundled) or IIS 10+

> If you prefer SQL Server: install SQL Server Express + SQL Server Management Studio and adapt the schema/connection string.

### Database Setup

1. Create a MySQL database, e.g., `signature_restaurant`.
2. Open `SQLQuery1.sql` in your DB client and **execute** it against `signature_restaurant` (creates tables and possibly seed data).
3. Create an application login/user if needed and grant privileges.

### Run

* **Debug (IIS Express):** Press **F5** (or ▶ Run).
* **IIS:** Create a site/app pointing the project’s folder; set app pool to .NET v4.0; update base URL in config if needed.

## Seed Data & Admin Access

* If no admin user exists after seeding, you can:

  * Use the **registration** form and manually elevate the user to `Admin` in the database, or
  * Insert a row into `Tbl_user` with `UserType='Admin'` and `IsBlock='No'`.
* Ensure passwords are stored according to your security policy (hashing is recommended before production use).

## Testing

The project follows layered testing:

* **Unit Testing:** Page‑level or controller/unit isolation (validate inputs, links, and processing logic).
* **Integration Testing:** After modules are unit‑tested, verify flows across modules (e.g., order → kitchen → invoice).
* **Validation/System Testing:** Confirm end‑to‑end requirements (input validation for dates/numerics/text, role permissions, data integrity).

Example validation rules (from documented test cases):

* Phone numbers must be **10 digits**; dates follow **DD/MM/YYYY**; numeric fields reject non‑numeric input; required fields cannot be blank.

## Reports & Analytics

* **Sales:** Item sales by day/week/month; popular vs. unpopular items.
* **Tables:** Occupancy by time windows.
* **Stock/Ingredients:** Totals, usage, availability; per‑item ingredient consumption.
* **Feedback/Reviews:** Ratings per item and qualitative feedback for QA.

## Limitations

* Coupons/discounts apply with **one active coupon policy** within configured ranges; no per‑customer coupon segmentation.
* Active default coupon cannot be applied if the **order amount exceeds** the coupon’s e‑coupon amount.
* **Digital payments** not implemented.
* Availability limited to locations where the restaurant operates.

## API/Config Notes (optional)

* Protect secrets using **User Secrets** (dev) or environment variables (prod).
* Centralize SMS/Email provider settings; add feature flags for OTP via email/SMS.

## Roadmap / Future Enhancements

* Enable **grading** features and richer **analytics dashboards**.
* Integrate **digital payments** and **online food ordering**.
* Improve **table reservation** UX (availability view, reminders).
* Desktop app integration and deeper internal operations modules.
* Harden security: password hashing, role claims, CSRF protection, input sanitization.
* CI/CD with GitHub Actions; automated test coverage.

## Project Planning (Process Notes)

* **Model:** Incremental (Waterfall applied in increments); each increment adds functionality and incorporates feedback.
* **Effort Distribution (guide):** Planning 2–3%, Analysis 10–25%, Design 20–25%, Coding 15–20%, Testing/Debugging 30–40%.
* **Artifacts:** Use‑case diagrams for Admin/Chef/Staff/Customer/Visitor; activity and sequence diagrams; navigation maps; schedule Gantt across 4 months.

---

### Acknowledgments

* Vivekanand College (B.C.A.) — guidance and support.
* Faculty & Project Guide for mentoring.
* Open‑source communities and documentation (ASP.NET MVC, MySQL, jQuery, CSS/SCSS).
