output "kube_config" {
  value     = azurerm_kubernetes_cluster.k8s.kube_config_raw
  sensitive = true
}

output "host" {
  value     = azurerm_kubernetes_cluster.k8s.kube_config.0.host
  sensitive = true
}

output "aks_identity" {
  value = azurerm_kubernetes_cluster.k8s.identity
}

resource "local_file" "kubeconfig" {
  depends_on = [azurerm_kubernetes_cluster.k8s]
  filename = "kubeconfig"
  content = azurerm_kubernetes_cluster.k8s.kube_config_raw
}
