# Delisc.io - A del.icio.us-ly 'aspired' Web App

Deliscio is a NextJS / .Net 9 web application, inspired by the once famous `del.icio.us-ly` social bookmarking site.

I am still really early into the development, so, please excuse any mess that you may encounter, or any issues.

## Technologies
- Frontend: NextJS / Tailwind / Shadcn
- API: .Net 9 Minimal API
- Backend: .Net 9
- Database: MongoDB
- AI: Ollama


## Getting Started
### Backend:
- Make sure to have Docker installed.
- Make sure that ./backend/AppHost is set as the Startup Project
- Click Run

- In the ./backend/Api/.http folder, there's a Submissions.http file. 
  This is a Postman-like file that you can use to populate the database with some links.

### Fronmtend:
- Make sure to have NodeJS installed.
- Iin ./frontend, run `npm install` to install the dependencies
- Then run `npm run dev` to start the frontend.

## Notes
- It does use Ollama (for tagging), and has a LLM already specified. 
  Depending on your system, or preference, you may want to change it. 
  To do so, open up AppHost's Program.cs



