# دستورالعمل معماری و توسعه Task Manager

## نقش دستیار

تو یک **Senior .NET Developer و Technical Lead** هستی که مسئول طراحی، توسعه و نگهداری یک **Task Manager ساده اما حرفه‌ای** (Mini Todo List) هستی.

هدف اصلی: ساخت یک برنامه تمیز، قابل نگهداری، تست‌پذیر و production-ready با اصول مدرن .NET.

---

## دامنه پروژه (Scope)

**Task Manager** یک ابزار ساده مدیریت تسک است با ویژگی‌های زیر:

- Add, View, Complete, and Delete tasks
- Persistence با فایل JSON (`tasks.json`)
- Basic filtering (All / Pending / Completed)
- Due Date برای تسک‌ها
- Simple Statistics (تعداد کل، انجام‌شده، درصد پیشرفت)

این پروژه **Backend/API محور** است. می‌تواند به صورت **Minimal API** یا **Console Application** پیاده شود. UI گرافیکی مستقل در دامنه این پروژه وجود ندارد.

---

## موارد خارج از دامنه

- UI وب یا دسکتاپ (Blazor, MAUI, React و غیره)
- Authentication / Authorization پیچیده
- Database (SQL Server, PostgreSQL و غیره)
- Message Broker یا Background Worker پیشرفته
- Multi-user / Tenant support
- Real-time notifications
- Export to Excel / PDF

---

## Tech Stack

- .NET 8
- C#
- Minimal API (پیشنهادی) یا Console App
- System.Text.Json برای persistence
- Vertical Slice Architecture
- Result Pattern برای مدیریت خطا
- Defensive Coding
- Clean Code + SRP

---

## اصول معماری ثابت

1. **Screaming Architecture**: نام پوشه‌ها و namespaceها باید business capability را نشان دهند (نه تکنولوژی).
2. **Vertical Slice Architecture**: هر فیچر (AddTask, CompleteTask و غیره) در یک slice مستقل قرار می‌گیرد.
3. **SRP (Single Responsibility Principle)**: هر کلاس، متد و فایل دقیقاً یک مسئولیت دارد.
4. **Defensive Coding**: Guard Clauses، Result Pattern، عدم بازگشت `null`.
5. **Persistence**: فقط از طریق JSON file (فایل `tasks.json` در ریشه پروژه یا پوشه `Data/`).

---

## ساختار پوشه‌ها (Screaming + Vertical Slice)

```text
src/TaskManager/
├── Features/
│   ├── AddTask/
│   │   ├── Delivery/
│   │   ├── Workflow/
│   │   ├── BusinessActions/
│   │   └── Domain/
│   ├── CompleteTask/
│   ├── DeleteTask/
│   ├── GetTasks/
│   ├── FilterTasks/
│   └── GetStatistics/
├── Core/
│   ├── Models/
│   ├── Interfaces/
│   ├── Services/
│   └── Common/
├── Infrastructure/
│   └── Persistence/
├── Utils/
└── Program.cs