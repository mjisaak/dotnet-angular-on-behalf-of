output "azure_app_service-frontend" {
  value = azurerm_app_service.frontend.id
}

output "azure_app_service_backend" {
  value = azurerm_app_service.backend.id
}

output "azure_ad_frontend_app" {
  value = azuread_application.frontend.application_id
}

output "azure_ad_backend_app" {
  value = azuread_application.backend.application_id
}

output "azure_ad_backend_app_id_uri" {
  value = azuread_application.backend.identifier_uris
}