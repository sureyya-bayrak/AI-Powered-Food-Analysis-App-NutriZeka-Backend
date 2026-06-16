
# NutriZeka - AI-Powered Mobile Food Analysis Platform

## 🚀 Overview
NutriZeka is an AI-supported, mobile-based decision support system designed to translate complex data found on packaged foods (such as E-codes, hidden sugars, and additives) into simplified, understandable information. The project's primary goal is to minimize the "cognitive load" consumers face during grocery shopping, especially those with specific health profiles like diabetes, gluten sensitivity, or a vegan diet.

## 💡 The Problem & Our Solution
Current market solutions often rely on static data and place a high cognitive load on consumers. Industry leaders like Yuka provide static scoring that is identical for every user[cite: 8]. 

NutriZeka differentiates itself by combining objective data discipline with a highly personalized feedback mechanism. Instead of unfairly lowering a product's general Nutri-Score due to a user's personal sensitivities, the system maintains the general scientific score while generating specific "Critical Warning Signs" and dynamic feedback tailored to the individual's health profile (e.g., gluten, lactose, palm oil).

## ✨ Key Features
* **Personalized Dynamic Scoring:** Delivers targeted warnings based on specific dietary restrictions rather than a one-size-fits-all static grade.
* **High-Reliability Barcode Scanning:** Strategically utilizes API-based barcode reading rather than Optical Character Recognition (OCR) to eliminate image processing errors and guarantee accurate data retrieval[cite: 7, 9].
* **Food Detective AI:** Integrates the Google Gemini API to transform the application from a static database into a dynamic smart assistant, providing users with personalized, natural-language analysis of products.
* **Better Alternatives:** Proactively recommends healthier product alternatives from the database to help users build better long-term eating habits.
* **Comprehensive Admin Panel:** A centralized dashboard built with ASP.NET Core MVC featuring CSV data import and dynamic listing for managing products and user profiles efficiently.

## 🏗️ Architecture & Technologies
The project is built with a focus on clean code, scalability, and security, utilizing modern software engineering practices.

### Backend (RESTful API)
* **Framework:** ASP.NET Core Web API 
* **Architecture:** Onion Architecture to minimize layer dependencies and ensure separation of concerns 
* **Design Pattern:** Repository Design Pattern for data access abstraction 
* **Database & ORM:** Microsoft SQL Server using Entity Framework Core with a Code-First approach 
* **Security:** JWT (JSON Web Token) based authentication for secure mobile-API communication, and Role-Based Authorization for the MVC Admin Panel 
* **Optimization:** DTO (Data Transfer Object) structures and AutoMapper integration for robust data security and network efficiency 
* **Documentation:** Swagger UI integrated for interactive endpoint testing 

### Frontend (Mobile App)
* **Framework:** Flutter & Dart for seamless cross-platform performance 
* **Design:** UI/UX prototyped via Figma, focusing strictly on cognitive load reduction 
* **Authentication:** Firebase Authentication and Google Sign-In for a smooth user onboarding experience 
* **Scanning:** `MobileScanner` library integrated for real-time, high-performance barcode capturing 

### Data Science & AI
* **Data Pipeline:** Custom Python scripts were utilized to extract a targeted dataset for the Turkish market via the Open Food Facts API. The raw data was then rigorously cleaned and prepared using Web Scraping and data mining techniques[cite: 6].
* **Generative AI:** Google Gemini API 

### Description (Warning)
This  readme file will update ( I will add project screenshots.]


