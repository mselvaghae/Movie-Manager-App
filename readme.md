# 📽️ **Movie Manager App**  

Bienvenue dans le projet **Movie Manager App** ! Ce projet a été réalisé en **C#** avec **.NET** et inclut une partie **API backend** et une partie **frontend Blazor**. Il a été développé sur une période d'un mois.  

---

## 🏗️ **Architecture du Projet**
Le projet est composé de deux parties principales :  
1. **API Backend** – basée sur ASP.NET Core  
2. **Frontend Blazor** – interface utilisateur  

---

## 🚀 **Fonctionnalités**  

### 🌐 **Backend API**  
Le backend est construit avec **ASP.NET Core** et utilise les technologies suivantes :  
- **JWT** pour l'authentification  
- **Entity Framework** pour la gestion de la base de données (SQLite)  
- **Injection de dépendances**  
- **Gestion d'erreurs** avec `try-catch` et codes HTTP appropriés (200, 404, 500, etc.)  
- **Configuration sécurisée** (Clé API OMDB, Secret JWT)  
- **Appels asynchrones** avec `async/await`  

#### **➡️ Controller User**  
✅ Récupérer la liste des utilisateurs (Id, Pseudo, Rôle)  
✅ Récupérer un utilisateur par son pseudo et mot de passe (login)  
✅ Ajouter un utilisateur (register)  
✅ Modifier un utilisateur (Pseudo, Password, Role)  
✅ Supprimer un utilisateur  
✅ Le mot de passe est hashé et n'est jamais renvoyé dans la réponse  

#### **➡️ Controller Favorite**  
✅ Récupérer les favoris d'un utilisateur  
✅ Ajouter un favori  
✅ Supprimer un favori  

#### **➡️ Controller Movie**  
✅ Récupérer la liste des films  
✅ Supprimer un film  

#### **➡️ Controller OMDB**  
✅ Rechercher un film par son titre  
✅ Importer des films depuis l'API OMDB  

#### **➡️ Services**  
✅ **JWT Service** – Gestion du token JWT  
✅ **OMDB Service** – Communication avec l'API OMDB  

---

### 🖥️ **Frontend Blazor**  
Le frontend est développé avec **Blazor** et permet une expérience utilisateur fluide et sécurisée.  

#### **➡️ Fonctionnalités Blazor**  
✅ **Formulaire de connexion**  
✅ **Formulaire d'inscription**  
✅ **Liste des films** affichée sous forme de cartes  
✅ **Ajouter/Supprimer** des films des favoris  
✅ **Liste des utilisateurs** avec leurs rôles  
✅ **Page des favoris**  
✅ **Page d'administration** pour importer des films  
✅ **Déconnexion**  

#### **➡️ Services Blazor**  
✅ `AuthService` – Gestion de l'authentification  
✅ `UserService` – Gestion des utilisateurs  
✅ `FavoriteService` – Gestion des favoris  
✅ `MovieService` – Gestion des films  

#### **➡️ Gestion du Token JWT**  
✅ Le token est stocké dans le **LocalStorage**  
✅ Le token est envoyé dans le header des requêtes API protégées  
✅ Les routes sont protégées en fonction du rôle de l'utilisateur  

---

## 🛠️ **Technologies utilisées**  
✅ **Langage** : C#  
✅ **Framework Backend** : .NET Core  
✅ **Base de Données** : SQLite  
✅ **Authentification** : JWT  
✅ **Appels API** : OMDB  
✅ **Frontend** : Blazor  

---

## 📥 **Installation**  
### **1. Cloner le dépôt**  
```bash
git clone https://github.com/votre-utilisateur/movie-manager-app.git
```
### **2. Configurer la base de données**
Configurer la chaîne de connexion dans le fichier appsettings.json :

```json

"ConnectionStrings": {
  "DefaultConnection": "Data Source=movies.db"
}
```
### **3. Configurer le token JWT et la clé API OMDB**
Dans appsettings.json :

```json
"JwtSettings": {
  "Secret": "VOTRE_SECRET_JWT",
  "Issuer": "https://localhost:5001",
  "Audience": "https://localhost:5001"
},
"OMDB": {
  "ApiKey": "VOTRE_CLÉ_OMDB"
}
```
### **4. Appliquer les migrations Entity Framework**
```bash
Copier
Modifier
dotnet ef database update
```
### **5. Lancer le projet**
Backend : dans TrackerFavorisApi

```bash
dotnet run 
```
Frontend : dans TrackerFavorisFront

```bash
dotnet run 
```
---
## ✅ **Utilisation**
1.Créez un compte via le formulaire d'inscription
2.Connectez-vous avec votre pseudo et mot de passe
3.Parcourez la liste des films et ajoutez-les à vos favoris
4.Accédez à la page admin pour importer des films
5.Gérez les utilisateurs et rôles via la page admin

## 📋 **Comptes Utilisateurs**
des utilisateurs qui sont déja exist 

| **Nom d'Utilisateur** | **Mot de Passe** | **Rôle** |
|:----------------------|:-----------------|:----------|
| `string`               | `string`         | `user`     |
| `azerty`               | `azerty`         | `user`     |
| `admin`                | `admin`          | `admin`    |

## 🔒**Sécurité**

- Les mots de passe sont hashés avant d'être stockés
- Les routes protégées nécessitent un token JWT valide
- Gestion des rôles pour protéger l'accès à certaines pages
  
## 📂 **Structure du Projet**
```pgsql
├── TrackerdeFavorisApi
│   ├── Controllers
│   ├── Models
│   ├── Services
│   └── Data
├── TrackerdeFavorisFront
│   ├── Components
│   ├── Pages
│   └── Services
├── appsettings.json
├── readme.md
```
## 🪪 **Licence**
Ce projet est sous licence MIT – voir le fichier LICENSE pour plus de détails.

## 🔗 **Liens utiles**

🌐 OMDB API : https://www.omdbapi.com
