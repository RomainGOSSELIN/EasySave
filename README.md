# EasySave
EasySave v1.0 est une application console développée en utilisant .NET Core, offrant une compatibilité étendue et une utilisation simple pour les utilisateurs.

Dans sa première version, le logiciel permet la création de jusqu'à cinq travaux de sauvegarde, chacun défini par un nom, un répertoire source, un répertoire cible et un type de sauvegarde (complet ou différentiel).

EasySave offre la possibilité d'exécuter les travaux de sauvegarde individuellement ou séquentiellement, selon les besoins de l'utilisateur. Tout cela est lancé via une ligne de commande.

Le logiciel garantit la sauvegarde de tous les éléments du répertoire source, y compris les fichiers et les sous-répertoires.

La première version du logiciel est traduite en français, anglais, allemand, italien et espagnol
# Commandes
Ce document présente l'ensemble des commandes qu'un utilisateur de l'application EasySave dans sa version 1.0 peut utiliser.

  - **run :**      Lancer un travail de sauvegarde
  - **create :**   Créer un nouveau travail de sauvegarde
  - **show :**     Afficher un travail de sauvegarde
  - **options :**  Régler les options (comme la langue)
  - **delete :**   Supprimer un travail de sauvegarde

Pour chacune de ces commandes, il existe différents arguments détaillés ci-dessous.

## Create
  - **-n**, **--name** (Obligatoire)                      Le nom du travil de sauvegarde
  - **-s**, **--source** (Obligatoire)                    La source du répertoire du travail de sauvegarde
  - **-d**, **--dest** (Obligatoire)                      La destination du répertoire du travail de sauvegarde
  - **-t**, **--type** <differential|full> (Obligatoire)  Pour choisir le type du travil de sauvegarde (**full** pour complet et **differential** pour différentiel)

## Run
  - **-j**, **--job** (Obligatoire)                      Le numéro du travail de sauvegarde
  
## Show
  - **-j**, **--job** (Obligatoire)                      Le numéro du travail de sauvegarde
  - **-a**, **--all**                                    Pour choisir tous les travaux de sauvegarde

## Delete 
  - **-j**, **--job** (Obligatoire)                      Le numéro du travail de sauvegarde
    
## Options
  - **-l**, **--lang <fr|en|de|it|es>**                  Pour choisir la langue
