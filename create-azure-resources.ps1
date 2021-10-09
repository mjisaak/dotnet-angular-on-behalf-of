$rg = 'rg-on-behalf-of-sample'
$location = 'germanywestcentral'
$appServiceplan = 'on-behalf-of-asp'
$frontendApp = 'on-behalf-of-frontend-web'
$backendApp = 'on-behalf-of-backend-web'

az group create --name $rg --location $location
az appservice plan create --name $appServicePlan --resource-group $rg --sku S1 --is-linux
az webapp create --resource-group $rg --plan $appServiceplan --name $frontendApp --runtime '"NODE|14-lts"' --deployment-local-git --query deploymentLocalGitUrl
az webapp create --resource-group $rg --plan $appServiceplan --name $backendApp --runtime '"DOTNET|6.0"' --deployment-local-git --query deploymentLocalGitUrl