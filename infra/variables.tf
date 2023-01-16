variable "node_count" {
  default = 2
}

variable "dns_prefix" {
  default = "aks-k8s-2023"
}

variable "cluster_name" {
  default = "aks-k8s-2023"
}

variable "kubernetes_version" {
  default = "1.21.2"
}

variable "acr_name" {
  default = "acrforaks2023"
}

variable "sql_name" {
  default = "mssql-2023"
}

variable "db_name" {
  default = "DB"
}

variable "db_admin_login" {
  default = "yosra"
}

variable "db_admin_password" {
  default = "yosra"
}

variable "storage_name" {
  default = "mssqlstorageaccount2023"
}

variable "resource_group_name" {
  default = "aks-k8s-2023"
}

variable "location" {
  default = "westeurope"
}
