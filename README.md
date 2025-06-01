# 📦 Windows Desktop Installer for .NET Applications

This is a fully functional **Windows desktop installer** project developed in **C# (.NET Framework)** with a beautiful **DevExpress UI**, designed to streamline the installation process of desktop applications, including SQL Server setup and database attachment.

## ✨ Features

- 🎨 Modern and elegant UI using **DevExpress components**
- 🔗 Downloads application files from a given URL
- ⚙️ Automatically installs dependencies (e.g., **SQL Server Express**)
- 🗃️ Attaches `.mdf` database files to the SQL Server instance
- 💡 Ideal for small business apps, local ERP/CRM installers, or internal tools

## 📂 How It Works

1. The installer downloads required setup files from a remote server.
2. It installs **SQL Server Express** silently if it's not already installed.
3. It attaches the provided SQL database automatically.
4. Then it installs the main desktop application.

## 🧰 Technologies Used

- C# (.NET Framework)
- DevExpress (UI Components)
- T-SQL for database management
- Windows Forms (WinForms)

## 🚀 Use Cases

- Standalone desktop app deployment
- Business software with local SQL Server
- Offline installers with automatic database setup

## 📦 Getting Started

Clone the repository and build the project using Visual Studio:

```bash
git clone https://github.com/yourusername/your-installer-project.git
