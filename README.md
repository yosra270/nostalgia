# nostalgia

## Infrastructure provisioning
I used Terraform to provision the required Azure service : resource group, kubernetes cluster, container registry, role assignment, firewall rule, ...
Terraform stores its state in a blob container on Azure.

## Packaging kubernetes manifests into a helm chart :
The kubernetes manifests include : the deployment of the .NET app along with its Loadbalancer service, the statefulset of MySQL, the secret containing the database password and the persistent volume and the persistent volume claim.

## Setting up datadog and argocd
I installed datadog and argocd agents inside the cluster using helm charts :
* Argocd : 
```
helm repo add argo https://argoproj.github.io/argo-helm

helm upgrade argocd argo/argo-cd --set server.service.type=LoadBalancer --namespace argo-system --install --create-namespace

kubectl port-forward service/argocd-server -n argo-system 8080:443
```

* Datadog : 
```
helm repo add datadog https://helm.datadoghq.com

helm install datadog -f datadog-values.yaml --set datadog.site='datadoghq.com'  --set datadog.apiKey=$API_KEY  --set datadog.apm.enabled=true  datadog/datadog
```

## Continuous Deployment using Argo CD :
Argo CD pulls the chart from this github repo. 
The default values in the chart can be overriden using ArgoCD.

## Monitoring
I used datadog to monitor the application and Serilog and OpenTelemetry to expose custom metrics (requests count metric), logs (enriched with attributes like : request_id, client_id, spand_id and trace_id) and tracings(with request_id + client_id).
