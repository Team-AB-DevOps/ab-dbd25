output "node_ip" {
  description = "Public IP address of the node"
  value       = hcloud_server.this.ipv4_address
}

output "ssh_command" {
  description = "SSH command to connect to the node"
  value       = "ssh root@${hcloud_server.this.ipv4_address}"
}
