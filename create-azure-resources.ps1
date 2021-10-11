$rg = 'rg-on-behalf-of-sample'
$location = 'germanywestcentral'
$appServiceplan = 'on-behalf-of-asp'
$frontendApp = 'on-behalf-of-frontend-web'
$backendApp = 'on-behalf-of-backend-web'

az group create --name $rg --location $location
az appservice plan create --name $appServicePlan --resource-group $rg --sku S1 --is-linux
az webapp create --resource-group $rg --plan $appServiceplan --name $frontendApp --runtime '"NODE|14-lts"' --startup-file "pm2 serve /home/site/wwwroot --no-daemon --spa"
az webapp config appsettings set --resource-group $rg --name $frontendApp --settings SCM_DO_BUILD_DURING_DEPLOYMENT=FALSE
az webapp create --resource-group $rg --plan $appServiceplan --name $backendApp --runtime '"DOTNET|6.0"' --deployment-local-git --query deploymentLocalGitUrl

# pm2 serve /home/site/wwwroot --no-daemon --spa 

# https://www.c-sharpcorner.com/article/easily-deploy-angular-app-to-azure-from-visual-studio-code/