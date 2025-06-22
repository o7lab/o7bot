<?php 
if (!class_exists('n0ise')) {
    class n0ise {
        private $mysql;
        private $connection;
        public $conn; // Public alias for internal connection
        public $admin_password = 'AcYQ%I9oaHnb'; // Change this in production
        public $online = 3600; // Default value for online duration (1 hour in seconds)

        // Declared properties to avoid dynamic creation warnings
        public array $commands = [];
        public int $online_bots = 0;
        public string $tpl = '';
        public $func = null;

        // Constructor: Initializes MySQL connection
        public function __construct() {
            $this->mysql = [
                'host' => 'localhost',
                'user' => 'n0ise',
                'pass' => '8b!49@9Bv4!@RzB',
                'db'   => 'n0ise'
            ];

            $this->db_connect();
        }

        // Establish DB connection
        private function db_connect() {
            $this->connection = new mysqli(
                $this->mysql['host'],
                $this->mysql['user'],
                $this->mysql['pass'],
                $this->mysql['db']
            );

            $this->conn = $this->connection;

            if ($this->connection->connect_error) {
                error_log("Database connection failed: " . $this->connection->connect_error);
                die("Database connection failed. Please try again later.");
            }
        }

        // Set new MySQL config and reconnect
        public function setMysqlConfig(array $config) {
            $this->mysql = $config;
            $this->db_connect(); // reconnect using new credentials
        }

        // Run a SQL query (prepared if $params is not empty)
        public function query($sql, $params = []) {
            if ($this->connection === null) {
                $this->db_connect();
            }

            if (!empty($params)) {
                $stmt = $this->connection->prepare($sql);
                if ($stmt === false) {
                    error_log("Query preparation failed: " . $this->connection->error);
                    return false;
                }

                $types = '';
                foreach ($params as $param) {
                    $types .= (is_int($param)) ? 'i' : 's';
                }

                $stmt->bind_param($types, ...$params);

                if (!$stmt->execute()) {
                    error_log("Query execution failed: " . $stmt->error);
                    return false;
                }

                return $stmt->get_result();
            } else {
                return $this->connection->query($sql);
            }
        }

        // Escape string for safe DB usage
        public function escape($string) {
            return $this->connection->real_escape_string($string);
        }

        // Get the last inserted ID
        public function insert_id() {
            return $this->connection->insert_id;
        }

        // Manually prepare a statement (for advanced use)
        public function prepare($sql) {
            if ($this->connection === null) {
                $this->db_connect();
            }
            return $this->connection->prepare($sql);
        }

        // Return MySQL config (for debug)
        public function getMysqlConfig() {
            return $this->mysql;
        }

        // Close DB connection
        public function closeConnection() {
            if ($this->connection) {
                $this->connection->close();
            }
        }
    }
}
?>
