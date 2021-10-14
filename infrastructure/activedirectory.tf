resource "azuread_application" "frontend" {
  display_name = var.azure_ad_frontend_app_name

  api {
    requested_access_token_version = 2

    known_client_applications = [
      azuread_application.backend.application_id,
    ]
  }

  web {
    homepage_url = "https://${var.app_service_frontend_name}"
    logout_url   = "https://${var.app_service_frontend_name}.azurewebsites.net/.auth/logout"
    redirect_uris = ["https://${var.app_service_frontend_name}.azurewebsites.net/.auth/login/aad/callback"]

    implicit_grant {
      access_token_issuance_enabled = true
      id_token_issuance_enabled = true
    }
  }
}

resource "azuread_application_password" "frontend" {
  application_object_id = azuread_application.frontend.object_id
  display_name          = var.azure_ad_frontend_app_name
}

resource "random_uuid" "azuread_application_backend" {}

resource "azuread_application" "backend" {
  display_name = var.azure_ad_backend_app_name
  identifier_uris = [ "http://${random_uuid.azuread_application_backend.result}" ]

  api {
    requested_access_token_version = 2

    oauth2_permission_scope {
      id                         = random_uuid.azuread_application_backend.result
      type                       = "Admin"
      value                      = "user_impersonation"
      admin_consent_display_name = "Access on-behalf-of-backend-web"
      admin_consent_description  = "Allow the application to access on-behalf-of-backend-web on behalf of the signed-in user."
      user_consent_display_name  = "Access on-behalf-of-backend-web"
      user_consent_description   = "Allow the application to access on-behalf-of-backend-web on your behalf."
    }
  }

  required_resource_access {
    resource_app_id = "00000003-0000-0000-c000-000000000000" # Microsoft Graph

    resource_access {
      id   = "df021288-bdef-4463-88db-98f22de89214" # User.Read.All
      type = "Role"
    }

    resource_access {
      id   = "b4e74841-8e56-480b-be8b-910348b18b4c" # User.ReadWrite
      type = "Scope"
    }
  }
}

resource "azuread_application_password" "backend" {
  application_object_id = azuread_application.backend.object_id
  display_name          = var.azure_ad_backend_app_name
}