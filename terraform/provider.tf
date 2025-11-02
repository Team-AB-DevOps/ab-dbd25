terraform {
  required_providers {
    hcloud = {
      source  = "hetznercloud/hcloud"
      version = "1.54.0"
    }
  }
}

provider "hcloud" {
  token = var.hcloud_token
}

locals {
  name_prefix = "dbd-25"
}
