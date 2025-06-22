<?php

class n0ise_func_logout {
	function run() {
		$_SESSION['loggedin'] = false;
		header("Location: index.php");
		die();
	}

}

?>