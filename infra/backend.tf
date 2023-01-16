terraform {
  backend "azurerm" {
    resource_group_name  = "resourcegrp1"
    storage_account_name = "projectstracc"
    container_name       = "remotestate"
    key                  = "prod.terraform.tfstate"
  }
}
