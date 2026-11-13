import {useState} from 'react';

function App() {
  const [weight, setWeight] = useState(60);
  
  return (
      <div className="bg-neutral-950 text-neutral-50 flex flex-col items-center justify-center min-h-screen p-4">
          <h1 className="text-2xl font-bold mb-4">Рабочий вес</h1>
          <h2 className="text-4xl font-bold mb-4">Вес: {weight} кг</h2>
          <div className="flex gap-3">
        <button className="bg-white text-black rounded-lg px-6 py-3"
            onClick={() => setWeight(weight + 2.5)}>
          +2.5
          </button>
        <button className="bg-white text-black rounded-lg px-6 py-3"
            onClick={() => {
            if (weight < 2.5) {return}
        setWeight(weight - 2.5);}}>
          -2.5
          </button>
      </div>
      </div>
  )
}

export default App;