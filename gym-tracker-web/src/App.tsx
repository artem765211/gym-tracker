import {useState} from 'react';
import {Button} from '@/components/ui/button';

function App() {
  const [weight, setWeight] = useState(60);
  
  return (
      <div className="bg-background text-foreground flex flex-col items-center justify-center min-h-screen p-4">
          <h1 className="text-2xl font-bold mb-4">Рабочий вес</h1>
          <h2 className="text-4xl font-bold mb-4">Вес: {weight} кг</h2>
          <div className="flex gap-3">
              <Button className="h-11 px-6 text-base"
                  onClick={() => setWeight(weight + 2.5)}>
          +2.5
              </Button>
              <Button className="h-11 px-6 text-base"
                  onClick={() => {
            if (weight < 2.5) {return}
        setWeight(weight - 2.5);}}>
          -2.5
          </Button>
      </div>
      </div>
  )
}

export default App;