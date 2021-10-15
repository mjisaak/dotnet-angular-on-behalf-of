variable "domain_name" {
  type    = string
  default = "whiteduck.de"
}

variable "default_location" {
  type    = string
  default = "germanywestcentral"
}

variable "resource_group_name" {
  type    = string
  default = "rg-on-behalf-of-sample"
}

variable "azure_ad_frontend_app_name" {
  type    = string
  default = "sku-on-behalf-of-frontend-web"
}

variable "azure_ad_backend_app_name" {
  type    = string
  default = "sku-on-behalf-of-backend-web"
}

variable "app_service_plan_name" {
  type    = string
  default = "sku-on-behalf-of-asp"
}

variable "app_service_frontend_name" {
  type    = string
  default = "sku-on-behalf-of-frontend-web"
}

variable "app_service_backend_name" {
  type    = string
  default = "sku-on-behalf-of-backend-web"
}

variable "graph_scopes" {
  default = ["User.Read"]
}