resource "hcloud_server" "this" {
  name         = "${local.name_prefix}-node"
  image        = "ubuntu-24.04"
  server_type  = "cx23"
  datacenter   = "fsn1-dc14" // Falkenstein, Tyskland
  firewall_ids = [hcloud_firewall.this.id]
  ssh_keys     = data.hcloud_ssh_keys.this.ssh_keys.*.name
  public_net {
    ipv4_enabled = true
    ipv6_enabled = true
  }
}

resource "hcloud_firewall" "this" {
  name = "${local.name_prefix}-firewall"

  rule {
    direction = "in"
    protocol  = "tcp"
    port      = "22" // SSH
    source_ips = [
      "0.0.0.0/0",
      "::/0"
    ]
  }

  // OUTBOUND
  rule {
    direction = "out"
    protocol  = "icmp"
    source_ips = [
      "0.0.0.0/0",
      "::/0"
    ]
  }

  rule {
    direction = "out"
    protocol  = "tcp"
    port      = "any"
    source_ips = [
      "0.0.0.0/0",
      "::/0"
    ]
  }

  rule {
    direction = "out"
    protocol  = "udp"
    port      = "any"
    source_ips = [
      "0.0.0.0/0",
      "::/0"
    ]
  }
}
