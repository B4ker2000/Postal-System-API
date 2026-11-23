# 📦 Postal System API & UI

A robust, event-sourced postal tracking system built with .NET 8 and a localized frontend. This system allows postmen to manage the lifecycle of a package from registration to final delivery using an audit-trail approach to determine package status and location.

---

## 🚀 Tech Stack

- **Backend:** C# .NET 8.0 Web API
- **Database:** SQL Server (with Entity Framework Core)
- **Frontend:** HTML5, CSS3 (Custom Properties), JavaScript (Vanilla)
- **Architecture:** Event-Sourcing for tracking and audit logs
- **Version Control:** GitHub

---

## ✨ Key Features

- **Event-Sourced Tracking:** Package status and location are not stored as static fields but are calculated dynamically from a sequence of events (e.g., Registered →→ Picked Up →→ Delivered).
- **Secure Delivery:** Delivery confirmation requires a unique 6-digit OTP generated at registration.
- **Multi-language Support (i18n):** Full English and Persian (RTL) support with a seamless language toggle.
- **Warehouse-Centric Logic:** Postmen are assigned to specific warehouses and can only interact with packages currently located at their assigned site.
- **Modern UI:**  A high-contrast "Midnight & Gold" theme with a responsive grid-based dashboard.
- **Audit Trail:**  Complete history logs for every package, providing a transparent chain of custody.

---

## 🛠️ Project Structure

```
postal-system-api/
├── src/
│   └── PostalSystem.Api/       # Backend API (Models, Services, Controllers)
├── PostalSystem.Tests/         # xUnit tests with Moq and InMemory EF Core
└── PostalSystem.UI/            # Frontend (HTML, CSS, JS)
```

## ⚙️ Installation & Setup

### Optional Seed Data

These data should be entered manually (They are simple dummy data to help you enter your own!):

```sql
-- Cities 
INSERT INTO Cities (CityName) VALUES (N'تبریز');
INSERT INTO Cities (CityName) VALUES ('東京');
INSERT INTO Cities (CityName) VALUES ('Москва');


-- Warehouses
INSERT INTO Warehouses (WarehouseName, CityId, IsActive)
VALUES (N'پست تبریز', 1, 1);

INSERT INTO Warehouses (WarehouseName, CityId, IsActive)
VALUES ('日本郵政公社', 2, 1);

INSERT INTO Warehouses (WarehouseName, CityId, IsActive)
VALUES ('Почта России', 3, 1);

-- Event Types (Code and Category should not be changed!)
INSERT INTO EventTypes (Code, EventTypeName, Description, Category) 
VALUES ('PR1', N'ثبت بسته', N'بستۀ جدید ثبت شد', 1); -- Registered

INSERT INTO EventTypes (Code, EventTypeName, Description, Category) 
VALUES ('PW1', '倉庫への配送', '荷物は倉庫に到着しました。', 1); -- in Warehouse

INSERT INTO EventTypes (Code, EventTypeName, Description, Category) 
VALUES ('PD1', 'Посылка прибыла на склад.', 'Посылка прибыла в пункт назначения.', 1); -- Delivered 

INSERT INTO EventTypes (Code, EventTypeName, Description, Category) 
VALUES ('PP2', 'Пакет чатдырылмасы', 'Почталјон бағламаны чатдырды.', 2); -- Picked up

INSERT INTO EventTypes (Code, EventTypeName, Description, Category) 
VALUES ('PC2', 'Paketi ləğv edin', 'Paket sistem tərəfindən ləğv edildi.', 2); -- Canceled

-- Postman
INSERT INTO Postmen (WarehouseId, Name, Phone, IsActive)
VALUES (1, N'سعید آذری', '0914-655-9855', 1);

INSERT INTO Postmen (WarehouseId, Name, Phone, IsActive)
VALUES (1, N'鳩凛', '03-1234-5678', 2);

INSERT INTO Postmen (WarehouseId, Name, Phone, IsActive)
VALUES (1, N'Борис Михайлович Чернов', '+7-912-345-67-89', 3);
```

### Backend

1. Clone the repository.
2. Update the `DefaultConnection` string in `appsettings.json` to point to your SQL Server instance.
3. Run the migrations to create the database:
```bash
dotnet ef database update
```
4. Start the API:
```bash
dotnet run --project src/PostalSystem.Api
```

### Frontend

1. Navigate to the `PostalSystem.UI` folder.
2. Open `index.html` in any modern web browser (or use a Live Server extension).

---

## 🛣️ API Workflow

1. **Authentication:** Postmen login with Name and Phone. The `PostmanId` is then passed via the `X-Postman-Id` request header in all subsequent calls.
2. **Registration:** A package is registered with a destination city and address. The system generates a `TrackingGuid` and an `OTP`.
3. **Pickup:** A postman picks up a package from their assigned warehouse.
4. **Transfer:** A postman at a different warehouse "accepts" the package into their facility.
5. **Delivery:** The postman enters the OTP to mark the package as delivered.
6. **Cancel:** Postmen can also cancel packages that were just created.
7. **Tracking:** Additionaly postmen can look up all the events of a package using that package's unique `TrackingGuid`.

---

## 🎨 UI Theme

The interface utilizes CSS custom properties for easy themeability, featuring (Default theme was inspired by the general theme used by the Wildhunt academy from Blue Archive which itself is heavily inspired from Harry Potter and other Wizardry series!):

- **Primary:** Deep Purples & Midnight Blacks
- **Accents:** Forest Green & Gold

---

## 📝 License 

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

- *B4ker2000*

## 🗒️ Notes

This project took 13 days in total to create the core functions, and another 3 days to add localization and more polish to it overall! 

**Special thanks** to my Seniors for tolerating me and giving me the original idea!

I'd appreciate any feedback and bug reports you've got! ***Thanks for taking a look at my project!!!***