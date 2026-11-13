import {useState} from 'react';

function App() {
  const [weight, setWeight] = useState(60);
  
  return (
      <div>
        <h1>Счетчик</h1>
        <button onClick={() => setWeight(weight + 2.5)}>
          +2.5
          </button>
        <button onClick={() => {
            if (weight < 2.5) {return}
        setWeight(weight - 2.5);}}>
          -2.5
          </button>
        <h2>Вес: {weight} кг</h2>
      </div>
  )
}

export default App;