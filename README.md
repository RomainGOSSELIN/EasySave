# EasySave
3A - CESI Toulouse

# Documentation v1.0 :


## Commandes :
  - **run :**      Lancer un travail de sauvegarde
  - **create :**   Créer un nouveau travail de sauvegarde
  - **show :**     Afficher un travail de sauvegarde
  - **options :**  Régler les options (comme la langue)
  - **delete :**   Supprimer un travail de sauvegarde

Pour chaque commande il existe des arguments différents : 

## Arguments commande Create : 
  - **-n**, **--name** (Obligatoire)                      Le nom du travil de sauvegarde
  - **-s**, **--source** (Obligatoire)                    La source du répertoire du travail de sauvegarde
  - **-d**, **--dest** (Obligatoire)                      La destination du répertoire du travail de sauvegarde
  - **-t**, **--type** <differential|full> (Obligatoire)  Pour choisir le type du travil de sauvegarde (**full** pour complet et **differential** pour différentiel)

## Arguments commande Run : 

## Arguments commande Show : 
  - **-j**, **--job**                                     Le numéro du travail de sauvegarde
  - **-a**, **--all**                                     Pour choisir tous les travaux de sauvegarde
## Arguments commande Delete : 

## Arguments commande Options : 


## Possibilité ajout variable d'environnement : 

