if ($_GET['action'] === 'debug') {
    // Make sure only admin or authorized users access this
    session_start();
    if (!isset($_SESSION['loggedin']) || $_SESSION['loggedin'] !== true) {
        header("Location: index.php?action=login");
        exit;
    }

    // Gather debug info
    $debugInfo = [];

    // Example: Show current session variables
    $debugInfo['session'] = $_SESSION;

    // Example: Show some server info
    $debugInfo['server'] = [
        'PHP Version' => phpversion(),
        'Loaded Extensions' => get_loaded_extensions(),
        'Memory Usage' => memory_get_usage(),
        'Current Time' => date('Y-m-d H:i:s'),
    ];

    // Optionally show custom logs if you have any (e.g., tail last lines of your error log)
    $logFile = '/var/log/apache2/error.log';
    if (file_exists($logFile)) {
        $debugInfo['last_log_lines'] = shell_exec("tail -n 20 " . escapeshellarg($logFile));
    } else {
        $debugInfo['last_log_lines'] = 'Log file not found.';
    }

    // Render debug info as HTML
    echo "<h2>Debug Info</h2>";

    foreach ($debugInfo as $key => $value) {
        echo "<h3>" . htmlspecialchars($key) . "</h3><pre>";
        if (is_array($value) || is_object($value)) {
            print_r($value);
        } else {
            echo htmlspecialchars($value);
        }
        echo "</pre>";
    }

    exit;
}
