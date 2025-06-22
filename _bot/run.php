<?php
if (session_status() === PHP_SESSION_NONE) {
    session_start();
}

// Include class definition
include("inc/n0ise.class.php");

// Create instance of n0ise class (which automatically connects to DB via the constructor)
$_n0ise = new n0ise();

// Include configuration and other necessary files
include("inc/config.inc.php");
include("inc/content.funcs.php");

// No need to call db_connect() here as it is now handled in the constructor of the n0ise class
?>
