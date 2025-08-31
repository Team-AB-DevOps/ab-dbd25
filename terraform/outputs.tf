output "droplet_ip" {
  description = "Public IP address of the droplet"
  value       = digitalocean_droplet.app-production.ipv4_address
}

output "reserved_ip" {
  description = "Reserved IP address"
  value       = digitalocean_reserved_ip.app-production-ip.ip_address
}

output "ssh_command" {
  description = "SSH command to connect to the droplet"
  value       = "ssh root@${digitalocean_reserved_ip.app-production-ip.ip_address}"
}
