resource "azurerm_app_service_plan" "asp" {
  name                = var.app_service_plan_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "Linux"
  reserved            = true
  sku {
    tier = "Standard"
    size = "S1"
  }
}

resource "azurerm_app_service" "frontend" {
  name                = var.app_service_frontend_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.asp.id

  site_config {
    app_command_line = "pm2 serve /home/site/wwwroot --no-daemon --spa"
    scm_type         = "None"
    linux_fx_version = "NODE|14-lts"
  }

  app_settings = {
    "SCM_DO_BUILD_DURING_DEPLOYMENT" = "false"
  }

  auth_settings {
    enabled                       = true
    unauthenticated_client_action = "RedirectToLoginPage"
    token_store_enabled           = true
    default_provider              = "AzureActiveDirectory"
    issuer                        = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/v2.0/"
    runtime_version               = "~v2" # https://github.com/hashicorp/terraform-provider-azurerm/issues/13591

    active_directory {
      client_id     = azuread_application.frontend.application_id
      client_secret = azuread_application_password.frontend.value
    }
  }
}

resource "azurerm_app_service" "backend" {
  name                = var.app_service_backend_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.asp.id

  site_config {
    scm_type                 = "None"
    dotnet_framework_version = "v6.0"
    linux_fx_version         = "DOTNET|6.0"

    cors {
      allowed_origins = [format("%s%s", "https://", azurerm_app_service.frontend.default_site_hostname)]
      support_credentials = true
    }
  }

  app_settings = {
    "AzureAd__Instance"     = "https://login.microsoftonline.com",
    "AzureAd__Domain"       = var.domain_name,
    "AzureAd__TenantId"     = data.azurerm_client_config.current.tenant_id,
    "AzureAd__ClientId"     = azuread_application.backend.application_id,
    "AzureAd__ClientSecret" = azuread_application_password.backend.value,
    "AzureAd__ClientIdUri"  = azuread_application.backend.application_id,
    "AzureAd__GraphScopes"  = join(", ", var.graph_scopes)
  }

  auth_settings {
    enabled                       = false
    token_store_enabled           = true
    default_provider              = "AzureActiveDirectory"
    issuer                        = "https://login.microsoftonline.com/${data.azurerm_client_config.current.tenant_id}/v2.0/"
    runtime_version               = "~v2" # https://github.com/hashicorp/terraform-provider-azurerm/issues/13591

    active_directory {
      client_id     = azuread_application.backend.application_id
      client_secret = azuread_application_password.backend.value
    }
  }
}