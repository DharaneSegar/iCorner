import { useState } from "react";
import { FiMessageSquare, FiShoppingCart, FiBarChart2 } from "react-icons/fi";
import { FaBoxOpen } from "react-icons/fa";

// Main Dashboard Component
const Dashboard = () => {
  return (
    <div className="h-screen w-full bg-gray-100 text-black">
      <Board />
    </div>
  );
};

// Board Component
const Board = () => {
  const [cards, setCards] = useState(DEFAULT_CARDS);

  return (
    <>
      <div className="flex justify-center">
        <h1 className="text-4xl font-bold mb-8 mt-2">Vendor Dashboard</h1>
      </div>
      <div className="flex h-full w-full gap-3 overflow-scroll p-2">
        <Column
          title="Orders"
          column="orders"
          headingColor="text-blue-500"
          icon={<FiShoppingCart />}
          cards={cards}
          setCards={setCards}
        />
        <Column
          title="Inventory"
          column="inventory"
          headingColor="text-green-500"
          icon={<FaBoxOpen />}
          cards={cards}
          setCards={setCards}
        />
        <Column
          title="Sales Analytics"
          column="analytics"
          headingColor="text-yellow-500"
          icon={<FiBarChart2 />}
          cards={cards}
          setCards={setCards}
        />
        <Column
          title="Messages"
          column="messages"
          headingColor="text-red-500"
          icon={<FiMessageSquare />}
          cards={cards}
          setCards={setCards}
        />
        {/* Make sure BurnBarrel component is defined or remove this */}
        {/* <BurnBarrel setCards={setCards} /> */}
      </div>
    </>
  );
};

// Column Component
const Column = ({ title, headingColor, icon, cards, column, setCards }) => {
  const [active, setActive] = useState(false);

  const handleDragStart = (e, card) => {
    e.dataTransfer.setData("cardId", card.id);
  };

  const handleDragEnd = (e) => {
    const cardId = e.dataTransfer.getData("cardId");
    setActive(false);
    clearHighlights();

    const indicators = getIndicators();
    const { element } = getNearestIndicator(e, indicators);
    const before = element.dataset.before || "-1";

    if (before !== cardId) {
      let copy = [...cards];
      let cardToTransfer = copy.find((c) => c.id === cardId);
      if (!cardToTransfer) return;

      cardToTransfer = { ...cardToTransfer, column };
      copy = copy.filter((c) => c.id !== cardId);

      const moveToBack = before === "-1";
      if (moveToBack) {
        copy.push(cardToTransfer);
      } else {
        const insertAtIndex = copy.findIndex((el) => el.id === before);
        if (insertAtIndex === undefined) return;
        copy.splice(insertAtIndex, 0, cardToTransfer);
      }

      setCards(copy);
    }
  };

  const handleDragOver = (e) => {
    e.preventDefault();
    highlightIndicator(e);
    setActive(true);
  };

  const clearHighlights = (els) => {
    const indicators = els || getIndicators();
    indicators.forEach((i) => {
      i.style.opacity = "0";
    });
  };

  const highlightIndicator = (e) => {
    const indicators = getIndicators();
    clearHighlights(indicators);
    const el = getNearestIndicator(e, indicators);
    el.element.style.opacity = "1";
  };

  const getNearestIndicator = (e, indicators) => {
    const DISTANCE_OFFSET = 50;
    const el = indicators.reduce(
      (closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = e.clientY - (box.top + DISTANCE_OFFSET);

        if (offset < 0 && offset > closest.offset) {
          return { offset: offset, element: child };
        } else {
          return closest;
        }
      },
      {
        offset: Number.NEGATIVE_INFINITY,
        element: indicators[indicators.length - 1],
      }
    );
    return el;
  };

  const getIndicators = () => {
    return Array.from(document.querySelectorAll(`[data-column="${column}"]`));
  };

  const handleDragLeave = () => {
    clearHighlights();
    setActive(false);
  };

  const filteredCards = cards.filter((c) => c.column === column);

  return (
    <div className="w-56 shrink-0">
      <div className="mb-3 flex items-center justify-between">
        <h3 className={`font-medium ${headingColor}`}>
          {icon} {title}
        </h3>
        <span className="rounded text-sm text-neutral-700">
          {filteredCards.length}
        </span>
      </div>
      <div
        onDrop={handleDragEnd}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        className={`h-full w-full transition-colors ${
          active ? "bg-neutral-800/50" : "bg-neutral-800/0"
        }`}
      >
        {filteredCards.map((c) => (
          <Card key={c.id} {...c} handleDragStart={handleDragStart} />
        ))}
        <DropIndicator beforeId={null} column={column} />
        <AddCard column={column} setCards={setCards} />
      </div>
    </div>
  );
};

// Card Component
const Card = ({ id, title, handleDragStart }) => {
  return (
    <div
      draggable
      onDragStart={(e) => handleDragStart(e, { id, title })}
      className="border rounded mb-2 p-2 bg-white shadow hover:shadow-lg"
    >
      {title}
    </div>
  );
};

// DropIndicator Component
const DropIndicator = ({ beforeId, column }) => {
  return <div data-before={beforeId} className="h-6 w-full bg-transparent" />;
};

// AddCard Component
const AddCard = ({ column, setCards }) => {
  const addNewCard = () => {
    const title = prompt("Enter card title");
    if (title) {
      const newCard = { title, id: Date.now().toString(), column };
      setCards((prev) => [...prev, newCard]);
    }
  };

  return (
    <button onClick={addNewCard} className="bg-blue-500 text-white p-2 rounded">
      Add Card
    </button>
  );
};

// Default Cards
const DEFAULT_CARDS = [
  { title: "Pending Order #12345", id: "1", column: "orders" },
  { title: "Process Order #54321", id: "2", column: "orders" },
  { title: "Low stock: Product A", id: "3", column: "inventory" },
  { title: "Add new product to catalog", id: "4", column: "inventory" },
  { title: "Weekly sales report", id: "5", column: "analytics" },
  { title: "Message from customer #67890", id: "6", column: "messages" },
];

export default Dashboard;
