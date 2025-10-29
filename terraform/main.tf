# ----- Droplet resources -----
resource "digitalocean_droplet" "app-production" {
  image  = "ubuntu-25-04-x64"
  name   = "app-production"
  region = "fra1"
  size   = "s-1vcpu-1gb"
  ssh_keys = [
    data.digitalocean_ssh_key.ali_laptop.id,
    data.digitalocean_ssh_key.ali_pc.id,
    data.digitalocean_ssh_key.brian_laptop.id
  ]
}


# ----- Network resources -----

# Create a reserved IP
resource "digitalocean_reserved_ip" "app-production-ip" {
  droplet_id = digitalocean_droplet.app-production.id
  region     = digitalocean_droplet.app-production.region
}

# Assign firewall rules
resource "digitalocean_firewall" "app-production" {
  name        = "app-production-firewall"
  depends_on  = [digitalocean_droplet.app-production]
  droplet_ids = [digitalocean_droplet.app-production.id]

  inbound_rule {
    protocol         = "tcp"
    port_range       = "22"
    source_addresses = ["0.0.0.0/0", "::/0"] # Developer addresses here
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "80"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "443"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "5432"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule { // man-sign-1
    protocol         = "tcp"
    port_range       = "3000"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule { // man-sign-1
    protocol         = "tcp"
    port_range       = "8080"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "icmp"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }
  outbound_rule {
    protocol              = "tcp"
    port_range            = "1-65535"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }
  outbound_rule {
    protocol              = "udp"
    port_range            = "1-65535"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

}

